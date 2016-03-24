namespace Klonamari
{
    public class KatamariModel
    {
        public int totalCollectibleObjects;
        public int collectedObjects;

        public KatamariModel(int totalCollectibleObjects)
        {
            Reset(totalCollectibleObjects);
            KatamariEventManager.OnAttach += CollectObject;
            KatamariEventManager.OnDetach += BrokenOff;
        }

        public void Reset(int totalCollectibleObjects)
        {
            collectedObjects = 0;
            this.totalCollectibleObjects = totalCollectibleObjects;
            KatamariEventManager.ObjectivesUpdated(this);
        }

        public void CollectObject(CollectibleObject collected)
        {
            ++collectedObjects;
            KatamariEventManager.ObjectivesUpdated(this);

            if (collectedObjects >= totalCollectibleObjects)
            {
                KatamariEventManager.Victory(this);
            }
        }

        public void BrokenOff(CollectibleObject broken)
        {
            --collectedObjects;
            KatamariEventManager.ObjectivesUpdated(this);
        }
    }
}
