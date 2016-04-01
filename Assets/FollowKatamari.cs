using UnityEngine;

namespace Klonamari
{
    public class FollowKatamari : MonoBehaviour
    {
        public Vector3 offset;

        public void UpdatePosition(Katamari katamari)
        {
            //TODO: update our position to follow the Katamari instance. we should rotate around it if the player turns.
            //TODO: zoom out/in depending on obstructions and the size of the Katamari
            transform.position = katamari.transform.position + Quaternion.Euler(0, katamari.rotationY, 0) * offset;

            transform.LookAt(katamari.transform);
        }
    }
}
