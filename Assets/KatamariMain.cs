using UnityEngine;

namespace Klonamari
{
    public class KatamariMain : MonoBehaviour //TODO: make this into a ScriptableObject?
    {
        //fine. we'll go with a singleton for now.
        private static KatamariMain Instance;
        public static KatamariMain GetInstance()
        {
            return Instance;
        }
        
#if UNITY_EDITOR || UNITY_STANDALONE
        public ControllerDetectionService controllerDetectionService = new XboxControllerDetectionService();
#endif

        //TODO: I hate everything about this singleton implementation.
        void Start()
        {
            Instance = this;
            model = new KatamariModel(spawner.toSpawn);
#if UNITY_EDITOR || UNITY_STANDALONE
            controllerDetectionService.Init(5.0f);
#endif
        }

        public KatamariModel model;
        public Katamari katamari;
        public CollectibleObjectSpawner spawner;
        
        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            controllerDetectionService.Tick(Time.deltaTime);
#endif
        }
    }
}
