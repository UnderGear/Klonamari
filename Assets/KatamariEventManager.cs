using UnityEngine;

namespace Klonamari
{
    public static class KatamariEventManager
    {

        public delegate void ContactAction(Collision collision);
        public static event ContactAction OnContact;
        
        public static void Contact(Collision collision)
        {
            if (OnContact != null)
            {
                OnContact(collision);
            }
        }

        public delegate void AttachAction(CollectibleObject collectible);
        public static event AttachAction OnAttach;

        public static void Attach(CollectibleObject collectible)
        {
            if (OnAttach != null)
            {
                OnAttach(collectible);
            }
        }

        public delegate void DetachAction(CollectibleObject collectible);
        public static event DetachAction OnDetach;

        public static void Detach(CollectibleObject collectible)
        {
            if (OnDetach != null)
            {
                OnDetach(collectible);
            }
        }

        public delegate void ObjectivesUpdatedAction(KatamariModel model);
        public static event ObjectivesUpdatedAction OnObjectivesUpdated;

        public static void ObjectivesUpdated(KatamariModel model)
        {
            if (OnObjectivesUpdated != null)
            {
                OnObjectivesUpdated(model);
            }
        }

        public delegate void VictoryAction(KatamariModel model);
        public static event VictoryAction OnVictory;

        public static void Victory(KatamariModel model)
        {
            if (OnVictory != null)
            {
                OnVictory(model);
            }
        }

        public delegate void InputChangedAction(KatamariInput input);
        public static event InputChangedAction OnInputChanged;

        public static void InputChanged(KatamariInput input)
        {
            if (OnInputChanged != null)
            {
                OnInputChanged(input);
            }
        }
    }
}
