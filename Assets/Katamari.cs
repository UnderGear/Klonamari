using UnityEngine;
using System.Collections.Generic;

namespace Klonamari
{
    public class Katamari : MonoBehaviour
    {
        public float ROLL_UP_MAX_RATIO = 0.25f;

        public float TORQUE_MULT = 1500.0f;
        public float FORCE_MULT = 500.0f;//400.0f;
        public float UPWARD_FORCE_MULT = 1000.0f;

        private KatamariInput katamariInput;
        public FollowKatamari follow;

        public Rigidbody rB;
        public SphereCollider sphere;
        private float volume;
        public float density;

        private List<Transform> touchingClimbables = new List<Transform>();

        public bool isGrounded;
        public int defaultContacts
        {
            get; private set;
        }

        private List<CollectibleObject> collectibles = new List<CollectibleObject>();
        private List<CollectibleObject> irregularCollectibles = new List<CollectibleObject>(); //TODO, maybe use a Dictionary. or sort this list as things are inserted.

        void OnEnable()
        {
            KatamariEventManager.OnContact += OnContact;
            KatamariEventManager.OnDetach += OnDetach;
        }

        void OnDisable()
        {
            KatamariEventManager.OnContact -= OnContact;
            KatamariEventManager.OnDetach -= OnDetach;
        }
        
        void Start()
        {
#if UNITY_EDITOR
            katamariInput = new KatamariKeyboardInput();
#endif
            //TODO: other input implementations for mobile, etc. we could also build a way for the user to select them once we have more.
            
            rB = GetComponent<Rigidbody>();
            sphere = GetComponent<SphereCollider>();
            volume = 4.0f / 3.0f * Mathf.PI * Mathf.Pow(sphere.radius, 3); //initial volume calculated by radius of the sphere.
            rB.mass = density * volume;
        }

        // Update is called once per frame
        void Update()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, sphere.radius + 0.01f);

            katamariInput.Update(this);

            follow.UpdatePosition(this);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 10)
            {
                return;
            }

            OnContact(collision);

            Collider hit = collision.rigidbody.GetComponent<Collider>();
            float targetTop = hit.bounds.extents.y + collision.transform.position.y;
            float sphereBottom = transform.position.y - sphere.radius;
            if (collision.gameObject.layer == 8 && targetTop > sphereBottom && sphereBottom + 2.15 * sphere.radius > targetTop) //allowing a little cheat on sphere radius
            {
                ++defaultContacts;
                touchingClimbables.Add(collision.transform);
                Debug.Log("default contacts: " + defaultContacts);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            //Debug.Log("exit");
            if (touchingClimbables.Contains(collision.transform))
            {
                --defaultContacts;
                touchingClimbables.Remove(collision.transform);
                Debug.Log("default contacts: " + defaultContacts);
            }
        }

        void OnContact(Collision collision)
        {
            Transform t = collision.transform;

            Debug.Log("hit. v: " + (collision.relativeVelocity.magnitude) + ", layer: " + collision.gameObject.layer);

            CollectibleObject collectible = t.GetComponent<CollectibleObject>();
            if (collectible)
            {
                if (collectible.rB.mass < rB.mass * ROLL_UP_MAX_RATIO)
                {
                    if (!collectible.collected)
                    {
                        //TODO: we should update our model, I guess. mass and uhh...diameter? changed.
                        //Debug.Log("attach");

                        collectible.collected = true;
                        collectible.gameObject.layer = 9;

                        t.parent = transform;

                        volume += collectible.volume;
                        rB.mass += collectible.volume * collectible.density;
                        RecalculateRadius();
                        collectibles.Add(collectible);
                        
                        if (collectible.IsIrregular(sphere.radius)) //irregular objects will modify how our controls work. it might actually need to be a function of scale compared to our radius.
                        {
                            Destroy(collectible.rB);
                            irregularCollectibles.Add(collectible);
                        }
                        else {
                            collectible.rB.detectCollisions = false;
                            collectible.rB.isKinematic = true;
                        }
                    }
                }
                else
                {
                    float magnitude = collision.relativeVelocity.magnitude;
                    while (magnitude >= 7.0f && collectibles.Count > 0)
                    {
                        CollectibleObject toRemove = collectibles[collectibles.Count - 1];
                        collectibles.RemoveAt(collectibles.Count - 1);

                        OnDetach(toRemove);
                        magnitude -= 4.0f;
                    }
                }
            }
        }

        void OnDetach(CollectibleObject detached)
        {
            Debug.Log("detach");
            volume -= detached.volume;
            RecalculateRadius();
            float detachedMass = detached.volume * detached.density;
            rB.mass -= detachedMass;

            detached.Detach(detachedMass, this);
            
            
            //TODO: we should update our model, I guess. mass and uhh...diameter? changed.
        }

        private void RecalculateRadius()
        {
            sphere.radius = Mathf.Pow((3 * volume) / (4 * Mathf.PI), (1.0f / 3.0f));
            int irregulars = irregularCollectibles.Count;
            for (int i = irregulars - 1; i >= 0; --i)
            {
                CollectibleObject collectible = irregularCollectibles[i];
                if (!collectible.IsIrregular(sphere.radius))
                {
                    irregularCollectibles.RemoveAt(i);

                    collectible.rB = collectible.gameObject.AddComponent<Rigidbody>();
                    collectible.rB.detectCollisions = false;
                    collectible.rB.isKinematic = true;
                    collectible.rB.mass = collectible.volume * collectible.density;
                }
            }
        }
    }
}