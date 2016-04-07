using UnityEngine;

namespace Klonamari
{
    public class DualThumbstickInput : KatamariInput
    {
        private Vector3 input = new Vector3();

        public Vector3 Update(Katamari katamari)
        {
            input.Set(0, 0, 0);

            float leftHorizontal = Input.GetAxis("Horizontal");
            float leftVertical = Input.GetAxis("Vertical");

            float rightHorizontal = Input.GetAxis("RightHorizontal");
            float rightVertical = Input.GetAxis("RightVertical");

            //roughly based on http://strategywiki.org/wiki/Katamari_Damacy/Controls
            //both sticks forward -> 0, 0, 1
            //both sticks back -> 0, 0, -1
            //both sticks left -> -1, 0, 0
            //both sticks right -> 1, 0, 0
            //left forward, right back -> 0, 1, 0
            //left back, right forward -> 0, -1, 0

            //TODO: add boosting mechanics? quick 180?
            
            if (leftVertical > 0 && rightVertical > 0 || leftVertical < 0 && rightVertical < 0) //forward and backwards movement
            {
                input.z = (leftVertical + rightVertical) / 2.0f;
            }            

            if (leftHorizontal > 0 && rightHorizontal > 0 || leftHorizontal < 0 && rightHorizontal < 0) //left and right movement
            {
                input.x = (leftHorizontal + rightHorizontal) / 2.0f;
            }

            if (leftVertical > rightVertical && leftVertical > 0) //turn right
            {
                input.y = (leftVertical - rightVertical) / 2.0f;
            }
            else if (rightVertical > leftVertical && rightVertical > 0) //turn left
            {
                input.y = -(rightVertical - leftVertical) / 2.0f;
            }

            return input;
        }
    }
}
