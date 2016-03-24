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

        private Coroutine fade;

        public void SetCollected(CollectibleObject collected)
        {
            collectedText.text = string.Format(collectedFormat, collected.displayName);
            //TODO: we should set up a spinning prefab for this guy, like in the actual game.
            if (fade != null)
            {
                StopCoroutine(fade);
            }
            fade = StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(timeoutSeconds);
            collectedText.text = "";
        }

        public void UpdateGoalText(KatamariModel model)
        {
            goalText.text = string.Format(goalTextFormat, model.collectedObjects, model.totalCollectibleObjects);
        }
    }
}
