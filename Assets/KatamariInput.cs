using UnityEngine;

namespace Klonamari
{
    public abstract class KatamariInput
    {
        protected Vector3 input = new Vector3();

        public abstract void Update(Katamari katamari);

        //TODO: let's do something with the awful camera. it needs to rotate about our y axis when we turn. forces need to be applied from its perspective.

        protected virtual void ProcessInput(Katamari katamari)
        {

            float forwardInputMultiplier = input.z * Time.deltaTime;
            float lateralInputMultiplier = input.x * Time.deltaTime;
            float upwardInputMultiplier = 0.0f;

            if ((Mathf.Abs(forwardInputMultiplier) > float.Epsilon || Mathf.Abs(lateralInputMultiplier) > float.Epsilon) && katamari.defaultContacts > 0)
            {
                //Debug.Log("up");
                upwardInputMultiplier += Time.deltaTime * katamari.UPWARD_FORCE_MULT; //* 1.0f, you know.
            }

            float adjustedTorqueMultiplier = katamari.TORQUE_MULT * katamari.rB.mass;
            float adjustedForceMultiplier =  katamari.rB.mass;
            if (!katamari.isGrounded)
            {
                adjustedForceMultiplier *= katamari.AIRBORNE_FORCE_MULT;
            } else
            {
                adjustedForceMultiplier *= katamari.FORCE_MULT;
            }
            //TODO: increase torque and force in proportion to mass.
            katamari.rB.AddTorque(forwardInputMultiplier * adjustedTorqueMultiplier, input.y * adjustedTorqueMultiplier * Time.deltaTime, -lateralInputMultiplier * adjustedTorqueMultiplier);
            katamari.rB.AddForce(lateralInputMultiplier * adjustedForceMultiplier, upwardInputMultiplier, forwardInputMultiplier * adjustedForceMultiplier);
        }
    }
}
