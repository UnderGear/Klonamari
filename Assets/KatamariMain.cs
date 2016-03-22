using UnityEngine;

namespace Klonamari
{
    public class KatamariMain : MonoBehaviour //TODO: probably make this into a ScriptableObject
    {
        //TODO: let's get global access to this guy. either service locator or singleton. DI is a bit heavy for this game.
        private static KatamariMain Instance;
        public static KatamariMain GetInstance()
        {
            return Instance;
        }

        //TODO: I hate everything about this singleton implementation.
        void Start()
        {
            Instance = this;
            model = new KatamariModel(spawner.toSpawn);
        }

        public KatamariModel model;
        public Katamari katamari;
        public CollectibleObjectSpawner spawner;

        //TODO: drive services if necessary
    }
}
