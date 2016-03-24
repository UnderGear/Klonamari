using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        void OnEnable()
        {
            KatamariEventManager.OnResetGame += Reset;
        }

        void OnDisable()
        {
            KatamariEventManager.OnResetGame -= Reset;
        }

        //TODO: I hate everything about this singleton implementation.
        void Start()
        {
            Instance = this;
            spawner.SpawnObjects();
            InitModels();
            InitServices();
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        public ControllerDetectionService controllerDetectionService = new XboxControllerDetectionService();
#endif

        public GameModel model;
        public Katamari katamari;
        public CollectibleObjectSpawner spawner;

        private void Reset()
        {
            KatamariEventManager.TearDown();
            SceneManager.LoadScene("test");
        }

        private void InitModels()
        {
            model = new GameModel(spawner.toSpawn);
        }

        private void InitServices()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            controllerDetectionService.Init(5.0f);
#endif
        }

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            controllerDetectionService.Tick(Time.deltaTime);
#endif
        }
    }
}
