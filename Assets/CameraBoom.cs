using UnityEngine;

namespace Klonamari
{
    public class CameraBoom : MonoBehaviour
    {
        public Vector3 boomDirection;
        public float BOOM_SCALE = 5.5f; //boom boom boom.

        private float orbitRadius;

        public void Init(Katamari katamari)
        {
            orbitRadius = BOOM_SCALE * katamari.sphere.radius;
        }

        public void UpdatePosition(Katamari katamari)
        {
            float targetDistance = BOOM_SCALE * katamari.sphere.radius;
            if (targetDistance > orbitRadius)
            {
                orbitRadius = Mathf.Lerp(orbitRadius, targetDistance, Time.deltaTime);
            }

            //rotate around the global y axis and offset by our scale * radius in the boomDirection. basically it just orbits the katamari, farther the larger the katamari is.
            transform.position = katamari.transform.position + Quaternion.Euler(0, katamari.rotationY, 0) * boomDirection.normalized * orbitRadius;

            transform.LookAt(katamari.transform);
        }
    }
}
