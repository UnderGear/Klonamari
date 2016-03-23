using UnityEngine;

namespace Klonamari
{
    public class KatamariJoystickInput : KatamariInput
    {
        public override void Update(Katamari katamari)
        {
            input.Set(0, 0, 0);

            float leftHorizontal = Input.GetAxis("Horizontal");
            float leftVertical = Input.GetAxis("Vertical");

            float rightHorizontal = Input.GetAxis("RightHorizontal");
            float rightVertical = Input.GetAxis("RightVertical");

            //TODO: let's set up our input vector.

            //both sticks forward -> 0, 0, 1
            //both sticks back -> 0, 0, -1
            //both sticks left -> -1, 0, 0
            //both sticks right -> 1, 0, 0
            //left forward, right back -> 0, 1, 0
            //left back, right forward -> 0, -1, 0

            //so our y component is based on the disparaity of our two vertical inputs and their magnitudes

            if (leftVertical > 0 && rightVertical > 0 || leftVertical < 0 && rightVertical < 0) //forward and backwards movement
            {
                input.z = (leftVertical + rightVertical) / 2.0f;
            }

            if (leftVertical > rightVertical) //turn right
            {
                input.y = (leftVertical - rightVertical) / 2.0f;
            } else if (rightVertical > leftVertical) //turn left
            {
                input.y = -(leftVertical - rightVertical) / 2.0f;
            }

            if (leftHorizontal > 0 && rightHorizontal > 0 || leftHorizontal < 0 && rightHorizontal < 0) //left and right movement
            {
                input.x = (leftHorizontal + rightHorizontal) / 2.0f;
            }

            Debug.Log("input: " + input.ToString());

            ProcessInput(katamari);
        }
    }
}
