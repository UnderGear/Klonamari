using UnityEngine;

namespace Klonamari
{
    public class KatamariUIMediator : MonoBehaviour
    {
        private KatamariUIView view;

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            view = gameObject.GetComponent<KatamariUIView>();
            EventManager.OnAttach += OnAttach;
            EventManager.OnObjectivesUpdated += OnObjectivesUpdated;
            EventManager.OnVictory += OnVictory;

            view.restartButton.onClick.AddListener(RestartClicked);
        }

        void OnDisable()
        {
            EventManager.OnAttach -= OnAttach;
            EventManager.OnObjectivesUpdated -= OnObjectivesUpdated;
            EventManager.OnVictory -= OnVictory;

            view.restartButton.onClick.RemoveListener(RestartClicked);
        }

        private void OnAttach(CollectibleObject attached, float diameter)
        {
            view.SetCollected(attached);
        }

        private void OnObjectivesUpdated(GameModel model)
        {
            view.UpdateGoalText(model.collectedObjects, model.totalCollectibleObjects);
        }

        private void OnVictory(GameModel model) //passing in model for now. seems likely that a real victory screen might show more stats, etc.
        {
            view.ShowVictory();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void RestartClicked()
        {
            view.HideVictory();
            EventManager.ResetGame();
        }
    }
}
