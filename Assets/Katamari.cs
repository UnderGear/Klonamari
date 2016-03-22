using UnityEngine;
using System.Collections.Generic;

namespace Klonamari
{
    public class Katamari : MonoBehaviour
    {
        public float ROLL_UP_MAX_RATIO = 0.25f; //NOTE that this isn't talking about rolling up stairs. the game's lingo uses this for collection.

        public float TORQUE_MULT = 1500.0f;
        public float FORCE_MULT = 500.0f;
        public float AIRBORNE_FORCE_MULT = 250.0f;
        public float UPWARD_FORCE_MULT = 1000.0f;
        public float STAIR_CLIMB_RATIO = 2.15f; // you can climb sheer walls STAIR_CLIMB_RATIO * radius above initial contact. if it's taller than that, you're falling down.

        private KatamariInput katamariInput;
        public FollowKatamari follow;

        public Rigidbody rB;
        public SphereCollider sphere;
        private float volume;
        public float density;
        public float mass { get; private set; }

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
            rB.mass = mass = density * volume;
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
            if (collision.gameObject.layer == 8 && targetTop > sphereBottom && sphereBottom + STAIR_CLIMB_RATIO * sphere.radius > targetTop) //allowing a little cheat on sphere radius
            {
                ++defaultContacts;
                touchingClimbables.Add(collision.transform);
                //Debug.Log("default contacts: " + defaultContacts);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            //Debug.Log("exit");
            if (touchingClimbables.Contains(collision.transform))
            {
                --defaultContacts;
                touchingClimbables.Remove(collision.transform);
                //Debug.Log("default contacts: " + defaultContacts);
            }
        }

        void OnContact(Collision collision)
        {
            Transform t = collision.transform;

            //Debug.Log("hit. v: " + (collision.relativeVelocity.magnitude) + ", layer: " + collision.gameObject.layer);

            CollectibleObject collectible = t.GetComponent<CollectibleObject>();
            if (collectible)
            {
                if (collectible.mass < mass * ROLL_UP_MAX_RATIO)
                {
                    if (!collectible.collected)
                    {
                        //TODO: we should update our model, I guess. mass and uhh...diameter? changed. notify that we collected the new object
                        //Debug.Log("attach");

                        collectible.collected = true;
                        collectible.gameObject.layer = 9;

                        t.parent = transform;

                        volume += collectible.volume;
                        mass += collectible.mass;
                        RecalculateRadius();
                        collectibles.Add(collectible);

                        Vector3 delta = (collectible.transform.position - transform.position);
                        float distance = delta.magnitude - sphere.radius;
                        Vector3 direction = delta.normalized;
                        collectible.transform.position = collectible.transform.position - direction * distance;

                        Debug.Log("distance: " + distance);
                        //TODO: drag the object closer so that the center is on the sphere's surface.
                        

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
            mass -= detached.mass;

            detached.Detach(this);
            
            
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

            //TODO: let's see if we should combine some meshes.
            int collectibleCount = collectibles.Count;
            if (collectibleCount >= 40)
            {
                CombineCollectibles();
            }
        }

        private void CombineCollectibles()
        {
            Debug.Log("combine");
            /*int collectibleCount = collectibles.Count;
            for (int i = collectibleCount - 1; i <= 0; --i)
            {
                CollectibleObject collectible = collectibles[i];
                collectibles.RemoveAt(i);

                
            }*/

            //TODO: combine?
            //GetComponent<Mesh>().CombineMeshes()
        }
    }
}