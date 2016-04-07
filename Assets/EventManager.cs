using UnityEngine;

namespace Klonamari
{
    public static class EventManager
    {
        //so this class got bloated with boilerplate stuff fast. I'd probably use StrangeIoC Signals for this sort of thing in a larger project.

        public delegate void ContactAction(Collision collision);
        public static event ContactAction OnContact;
        
        public static void Contact(Collision collision)
        {
            if (OnContact != null)
            {
                OnContact(collision);
            }
        }

        public delegate void AttachAction(CollectibleObject collectible, float diameter);
        public static event AttachAction OnAttach;

        public static void Attach(CollectibleObject collectible, float diameter)
        {
            if (OnAttach != null)
            {
                OnAttach(collectible, diameter);
            }
        }

        public delegate void DetachAction(CollectibleObject collectible, float diameter);
        public static event DetachAction OnDetach;

        public static void Detach(CollectibleObject collectible, float diameter)
        {
            if (OnDetach != null)
            {
                OnDetach(collectible, diameter);
            }
        }

        public delegate void ObjectivesUpdatedAction(GameModel model);
        public static event ObjectivesUpdatedAction OnObjectivesUpdated;

        public static void ObjectivesUpdated(GameModel model)
        {
            if (OnObjectivesUpdated != null)
            {
                OnObjectivesUpdated(model);
            }
        }

        public delegate void VictoryAction(GameModel model);
        public static event VictoryAction OnVictory;

        public static void Victory(GameModel model)
        {
            if (OnVictory != null)
            {
                OnVictory(model);
            }
        }

        public delegate void ResetGameAction();
        public static event ResetGameAction OnResetGame;

        public static void ResetGame()
        {
            if (OnResetGame != null)
            {
                OnResetGame();
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

        public static void TearDown()
        {
            OnInputChanged = null;
            OnResetGame = null;
            OnVictory = null;
            OnObjectivesUpdated = null;
            OnDetach = null;
            OnAttach = null;
            OnContact = null;
        }
    }
}
