using UnityEngine;

namespace Klonamari
{
    public class KatamariUIMediator : MonoBehaviour
    {
        private KatamariUIView view;

        void OnEnable()
        {
            view = gameObject.GetComponent<KatamariUIView>();
            KatamariEventManager.OnAttach += OnAttach;
            KatamariEventManager.OnObjectivesUpdated += OnObjectivesUpdated;
        }

        void OnDisable()
        {
            KatamariEventManager.OnAttach -= OnAttach;
            KatamariEventManager.OnObjectivesUpdated -= OnObjectivesUpdated;
        }

        private void OnAttach(CollectibleObject attached)
        {
            view.SetCollected(attached);
        }

        private void OnObjectivesUpdated(KatamariModel model)
        {
            view.UpdateGoalText(model);
        }
    }
}
