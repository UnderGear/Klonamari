using System.Collections;
using UnityEngine;

namespace Klonamari
{
    public class CollectibleObject : MonoBehaviour
    {
        public bool collected;
        public Rigidbody rB;
        public BoxCollider uncollectedCollider;
        public Collider collectedCollider; //intended to be a rounded collider

        public AudioClip[] collectionClips;
        public AudioClip detachClip;

        public float volume { get; private set; } //honestly, volume should probably be calculated, depending on the mesh we're using. maybe just collider bounds size.
        public float density;
        public float mass { get; private set; }
        public string displayName;

        void Start()
        {
            Vector3 size = transform.lossyScale;
            volume = size.x * size.y * size.z;
            rB.mass = mass = volume * density;
        }

        public void Attach(Katamari katamari)
        {
            uncollectedCollider.enabled = false;
            collectedCollider.enabled = true;
        }

        public void Detach(Katamari katamari)
        {
            transform.parent = null;
            gameObject.layer = 8;
            if (rB == null)
            {
                rB = gameObject.AddComponent<Rigidbody>();
            }
            rB.mass = mass;
            rB.isKinematic = false;
            rB.detectCollisions = true;

            //arbitrary explosion force to send stuff flying away a bit
            rB.AddExplosionForce(400.0f, katamari.transform.position, katamari.sphere.radius, 50.0f);

            collectedCollider.enabled = false;
            uncollectedCollider.enabled = true;

            StartCoroutine(DoEnableCollect());
        }

        private IEnumerator DoEnableCollect()
        {
            yield return new WaitForSeconds(1.0f);
            collected = false;
        }

        public bool IsIrregular(float radius)
        {
            float magnitude = transform.lossyScale.magnitude;
            return radius < magnitude;
        }

        public MeshFilter GetMeshFilter()
        {
            return GetComponent<MeshFilter>();
        }

        public Material GetMaterial()
        {
            return GetComponent<MeshRenderer>().material;
        }

        public AudioClip GetRandomCollectAudio()
        {
            int audioCount = collectionClips.Length;
            if (audioCount == 0)
            {
                return null;
            }
            return collectionClips[Random.Range(0, audioCount)];
        }
    }
}