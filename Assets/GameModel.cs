namespace Klonamari
{
    public class GameModel
    {
        public int totalCollectibleObjects;
        public int collectedObjects;
        public float diameter; //not actually used right now. the real Katamari has diameter goals instead of # collected objects.

        public GameModel(int totalCollectibleObjects, float diameter)
        {
            this.totalCollectibleObjects = totalCollectibleObjects;
            this.diameter = diameter;
            collectedObjects = 0;
            EventManager.OnAttach += CollectObject;
            EventManager.OnDetach += BrokenOff;

            EventManager.ObjectivesUpdated(this);
        }

        public void CollectObject(CollectibleObject collected, float diameter)
        {
            ++collectedObjects;
            this.diameter = diameter;

            EventManager.ObjectivesUpdated(this);

            if (collectedObjects >= totalCollectibleObjects)
            {
                EventManager.Victory(this);
            }
        }

        public void BrokenOff(CollectibleObject broken, float diameter)
        {
            --collectedObjects;
            this.diameter = diameter;

            EventManager.ObjectivesUpdated(this);
        }
    }
}
