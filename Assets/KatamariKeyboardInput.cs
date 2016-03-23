using UnityEngine;

namespace Klonamari
{
    public class KatamariKeyboardInput : KatamariInput
    {
        public override void Update(Katamari katamari)
        {
            input.Set(0, 0, 0);

            input.x = Input.GetAxis("Horizontal");

            input.z = Input.GetAxis("Vertical");

            input.Normalize();

            input.y = Input.GetAxis("Rotate");

            ProcessInput(katamari);
        }
    }
}
