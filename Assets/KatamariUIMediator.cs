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
            KatamariEventManager.OnVictory += OnVictory;

            view.restartButton.onClick.AddListener(RestartClicked);
        }

        void OnDisable()
        {
            KatamariEventManager.OnAttach -= OnAttach;
            KatamariEventManager.OnObjectivesUpdated -= OnObjectivesUpdated;
            KatamariEventManager.OnVictory -= OnVictory;

            view.restartButton.onClick.RemoveListener(RestartClicked);
        }

        private void OnAttach(CollectibleObject attached)
        {
            view.SetCollected(attached);
        }

        private void OnObjectivesUpdated(GameModel model)
        {
            view.UpdateGoalText(model);
        }

        private void OnVictory(GameModel model)
        {
            view.ShowVictory(model);
        }

        private void RestartClicked()
        {
            view.HideVictory();
            KatamariEventManager.ResetGame();
        }
    }
}
