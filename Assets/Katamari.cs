using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Klonamari
{
    public class Katamari : MonoBehaviour
    {
        private const float ONE_THIRD = 1.0f / 3.0f;

        public float ROLL_UP_MAX_RATIO = 0.25f; //NOTE that this isn't talking about rolling up stairs. the game's lingo uses this for collection.

        public float TORQUE_MULT = 1500.0f;
        public float FORCE_MULT = 500.0f;
        public float AIRBORNE_FORCE_MULT = 250.0f;
        public float UPWARD_FORCE_MULT = 1000.0f;
        public float STAIR_CLIMB_RATIO = 2.15f; // you can climb sheer walls STAIR_CLIMB_RATIO * radius above initial contact. if it's taller than that, you're falling down.
        public float BREAK_OFF_THRESHOLD = 10.0f;

        public float ROTATION_MULTIPLIER;

        private KatamariInput katamariInput;
        public CameraBoom follow;
        public AudioSource audioSource;

        public Rigidbody rB;
        public SphereCollider sphere;
        public float volume { get; private set; }
        public float density;
        public float mass { get; private set; }

        private List<Transform> touchingClimbables = new List<Transform>();

        public bool isGrounded { get; private set; }
        public int defaultContacts
        {
            get; private set;
        }

        public float rotationY
        {
            get; private set;
        }

        private List<CollectibleObject> collectibles = new List<CollectibleObject>();
        private List<CollectibleObject> irregularCollectibles = new List<CollectibleObject>();

        private Vector3 userInput = Vector3.zero;

        void OnEnable()
        {
            EventManager.OnInputChanged += SetInput;
        }

        void OnDisable()
        {
            EventManager.OnInputChanged -= SetInput;
        }

        void Start()
        {
            //A note here, I'd rather pull all of this stuff up into a Context class and keep all platform-dependent compilation up there.
            //the Context class could fill out either a Service Locator or set up bindings for DI. This class would just ask for an instance
            //of KatamariInput from injection or from the locator instead of calling new here.
#if UNITY_EDITOR || UNITY_STANDALONE
            SetInput(new KeyboardInput());
#elif UNITY_XBOX360 || UNITY_XBOXONE
            SetInput(new KatamariJoystickInput());
#endif
            //other input implementations for mobile, joystick, eye tracking, etc. we could also build a way for the user to select them once we have more.

            volume = 4.0f / 3.0f * Mathf.PI * Mathf.Pow(sphere.radius, 3); //initial volume calculated by radius of the sphere.
            Debug.Log("volume: " + volume + ", density: " + density);
            rB.mass = mass = density * volume;

            follow.Init(this);
        }

        private void SetInput(KatamariInput input)
        {
            katamariInput = input;
        }
        
        private void ProcessInput(Vector3 input)
        {
            float forwardInputMultiplier = input.z;
            float lateralInputMultiplier = input.x;
            float upwardInputMultiplier = 0.0f;

            //add an upward force if we're in contact with something we can climb.
            if ((Mathf.Abs(forwardInputMultiplier) > float.Epsilon || Mathf.Abs(lateralInputMultiplier) > float.Epsilon) && defaultContacts > 0)
            {
                upwardInputMultiplier += UPWARD_FORCE_MULT * mass;
            }

            float adjustedTorqueMultiplier = TORQUE_MULT * mass;
            float adjustedForceMultiplier = mass * (isGrounded ? FORCE_MULT : AIRBORNE_FORCE_MULT);
            Vector3 currentForward = new Vector3(0, rotationY, 0);

            Vector3 torque = new Vector3(forwardInputMultiplier * adjustedTorqueMultiplier, input.y * adjustedTorqueMultiplier, -lateralInputMultiplier * adjustedTorqueMultiplier);
            Vector3 force = new Vector3(lateralInputMultiplier * adjustedForceMultiplier, upwardInputMultiplier, forwardInputMultiplier * adjustedForceMultiplier);
            rB.AddTorque(Quaternion.Euler(currentForward) * torque);
            rB.AddForce(Quaternion.Euler(currentForward) * force);
        }
        
        void FixedUpdate()
        {
            ProcessInput(userInput);
        }

        void Update()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, sphere.radius + 0.01f);

            userInput = katamariInput.Update(this);
            rotationY += userInput.y * Time.deltaTime * ROTATION_MULTIPLIER;
            
            follow.UpdatePosition(this);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 10)
            {
                return;
            }

            bool rolledUp = OnContact(collision);

            if (!rolledUp)
            {
                Collider hit = collision.rigidbody.GetComponent<Collider>();
                float targetTop = hit.bounds.extents.y + collision.transform.position.y;
                float sphereBottom = transform.position.y - sphere.radius;
                if (collision.gameObject.layer == 8 && targetTop > sphereBottom && sphereBottom + STAIR_CLIMB_RATIO * sphere.radius > targetTop)
                {
                    ++defaultContacts;
                    touchingClimbables.Add(collision.transform);
                }
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (touchingClimbables.Contains(collision.transform))
            {
                --defaultContacts;
                touchingClimbables.Remove(collision.transform);
            }
        }

        private bool OnContact(Collision collision)
        {
            bool rolledUp = false;
            Transform t = collision.transform;

            CollectibleObject collectible = t.GetComponent<CollectibleObject>();
            if (collectible)
            {
                if (collectible.mass < mass * ROLL_UP_MAX_RATIO)
                {
                    if (!collectible.collected)
                    {
                        AudioClip clip = collectible.GetRandomCollectAudio();
                        if (clip)
                        {
                            audioSource.PlayOneShot(clip);
                        }
                        //collect the thing we hit!
                        collectible.collected = true;
                        rolledUp = true;
                        collectible.gameObject.layer = 9;

                        collectible.Attach(this);
                        t.parent = transform;

                        volume += collectible.volume;
                        mass += collectible.mass;
                        rB.mass += collectible.mass;
                        RecalculateRadius();

                        collectibles.Add(collectible);

                        Vector3 delta = (collectible.transform.position - transform.position);
                        float distance = delta.magnitude - sphere.radius;
                        Vector3 direction = delta.normalized;
                        
                        collectible.transform.position = collectible.transform.position - direction * distance;
                        
                        EventManager.Attach(collectible, sphere.radius * 2);

                        //large or irregular objects will make our katamari bounce differently until it grows large enough
                        if (collectible.IsIrregular(sphere.radius))
                        {
                            Destroy(collectible.rB);
                            irregularCollectibles.Add(collectible);
                        }
                        else {
                            
                            collectible.rB.mass = 0;
                            collectible.rB.detectCollisions = false;
                            collectible.rB.isKinematic = true;
                        }
                    }
                }
                else
                {
                    //decide how many objects to break off, then break them off.
                    float magnitude = collision.relativeVelocity.magnitude;
                    while (magnitude >= BREAK_OFF_THRESHOLD && collectibles.Count > 0)
                    {
                        CollectibleObject toRemove = collectibles[collectibles.Count - 1];
                        OnDetach(toRemove);
                        magnitude -= 4.0f;
                    }
                }
            }
            return rolledUp;
        }

        void OnDetach(CollectibleObject detached)
        {
            audioSource.PlayOneShot(detached.detachClip);
            //this could be improved by using a Dictionary and adding some sort of id to collectibles.
            collectibles.Remove(detached);
            if (irregularCollectibles.Contains(detached))
            {
                irregularCollectibles.Remove(detached);
            }

            if (!detached.IsIrregular(sphere.radius))
            {
                rB.mass -= detached.mass;
            }
            mass -= detached.mass;
            volume -= detached.volume;
            RecalculateRadius();

            detached.Detach(this);

            EventManager.Detach(detached, sphere.radius * 2);
        }

        private void RecalculateRadius()
        {
            sphere.radius = Mathf.Pow((3 * volume) / (4 * Mathf.PI), ONE_THIRD);

            //check to see if we're big enough for irregular objects to stop making us bounce irregularly
            int irregulars = irregularCollectibles.Count;
            for (int i = irregulars - 1; i >= 0; --i)
            {
                CollectibleObject collectible = irregularCollectibles[i];
                if (!collectible.IsIrregular(sphere.radius))
                {
                    Debug.Log("irregular rolled in");
                    irregularCollectibles.RemoveAt(i);

                    if (collectible.rB == null)
                    {
                        collectible.rB = collectible.gameObject.AddComponent<Rigidbody>();
                    }
                    collectible.rB.mass = 0;
                    collectible.rB.detectCollisions = false;
                    collectible.rB.isKinematic = true;
                }
            }
        }
    }
}