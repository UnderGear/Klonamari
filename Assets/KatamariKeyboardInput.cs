using UnityEngine;

namespace Klonamari
{
    public class KatamariKeyboardInput : KatamariInput
    {
        //TODO: torque probably needs to be updated as the mass of the Katamari changes.
        //TODO: should we switch from applying torque to just applying forces to the outside of the sphere?
       
        public override void Update(Katamari katamari) //we need to scale our forces based on our mass.
        {
            /*if (!katamari.isGrounded && katamari.defaultContacts <= 0)
            {
                return;
            }*/

            input.Set(0, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                input.z += 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                input.z -= 1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                input.x -= 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                input.x += 1;
            }

            input.Normalize();

            if (Input.GetKey(KeyCode.Q))
            {
                input.y += 1;
            }

            if (Input.GetKey(KeyCode.E))
            {
                input.y -= 1;
            }

            ProcessInput(katamari);
        }
    }
}
