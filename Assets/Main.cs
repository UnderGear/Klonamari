using UnityEngine;
using UnityEngine.SceneManagement;

namespace Klonamari
{
    public class Main : MonoBehaviour //TODO: make this into a ScriptableObject?
    {
        //fine. we'll go with a singleton for now.
        private static Main Instance;
        public static Main GetInstance()
        {
            return Instance;
        }
        
        void OnEnable()
        {
            EventManager.OnResetGame += Reset;
        }

        void OnDisable()
        {
            EventManager.OnResetGame -= Reset;
        }

        void Start()
        {
            Instance = this; //looks like nobody better try to reference Main from another MonoBehaviour's Start method!
            spawner.SpawnObjects();
            model = new GameModel(spawner.toSpawn, katamari.sphere.radius * 2);
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
            EventManager.TearDown();
            SceneManager.LoadScene("test");
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
