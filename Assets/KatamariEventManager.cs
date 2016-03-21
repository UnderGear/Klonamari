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

        public delegate void DetachAction(CollectibleObject collectible);
        public static event DetachAction OnDetach;

        public static void Detach(CollectibleObject collectible)
        {
            if (OnDetach != null)
            {
                OnDetach(collectible);
            }
        }
    }
}
