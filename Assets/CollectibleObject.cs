using System.Collections;
using UnityEngine;

namespace Klonamari
{
    public class CollectibleObject : MonoBehaviour
    {
        public bool collected;
        public Rigidbody rB;
        
        public float volume; //honestly, volume should probably be calculated, depending on the mesh we're using. maybe just collider bounds size.
        public float density;

        public void Detach(float mass, Katamari katamari)
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

            rB.AddExplosionForce(400.0f, katamari.transform.position, katamari.sphere.radius, 50.0f);

            StartCoroutine(DoEnableCollect());
        }

        private IEnumerator DoEnableCollect()
        {
            yield return new WaitForSeconds(1.0f);
            collected = false;
        }

        public bool IsIrregular(float radius)
        {
            float magnitude = GetComponent<Collider>().bounds.size.magnitude;
            return radius < magnitude;
        }
    }
}