namespace Klonamari
{
    public class GameModel
    {
        public int totalCollectibleObjects;
        public int collectedObjects;

        public GameModel(int totalCollectibleObjects)
        {
            this.totalCollectibleObjects = totalCollectibleObjects;
            collectedObjects = 0;
            KatamariEventManager.OnAttach += CollectObject;
            KatamariEventManager.OnDetach += BrokenOff;

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
