using UnityEngine;

namespace Klonamari
{
    public class KeyboardInput : KatamariInput
    {
        private Vector3 input = new Vector3();

        public Vector3 Update(Katamari katamari)
        {
            input.Set(0, 0, 0);

            input.x = Input.GetAxis("Horizontal");

            input.z = Input.GetAxis("Vertical");

            input.Normalize();

            input.y = -Input.GetAxis("Rotate");

            return input;
        }
    }
}
