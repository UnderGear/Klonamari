using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Klonamari
{
    [RequireComponent(typeof(KatamariUIMediator))]
    public class KatamariUIView : MonoBehaviour
    {
        public float timeoutSeconds = 5.0f;

        public Text collectedText;
        public string collectedFormat = "Collected: {0}";

        public Text goalText;
        public string goalTextFormat = "{0}/{1}";

        public GameObject victoryRoot;
        public Button restartButton;

        public CollectedObjectDisplay collectedObjectDisplay;

        private Coroutine fade;

        public void SetCollected(CollectibleObject collected)
        {
            collectedText.text = string.Format(collectedFormat, collected.displayName);
            
            collectedObjectDisplay.Init(collected.GetMeshFilter().sharedMesh, collected.GetMaterial(), collected.transform.localScale);

            if (fade != null)
            {
                StopCoroutine(fade);
            }
            fade = StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(timeoutSeconds);

            //TODO: clear the collected object
            collectedObjectDisplay.Clear();
            collectedText.text = "";
        }

        public void UpdateGoalText(GameModel model)
        {
            goalText.text = string.Format(goalTextFormat, model.collectedObjects, model.totalCollectibleObjects);
        }

        public void ShowVictory(GameModel model)
        {
            victoryRoot.SetActive(true);
        }

        public void HideVictory()
        {
            victoryRoot.SetActive(false);
        }
    }
}
