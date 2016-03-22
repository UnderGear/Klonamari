using UnityEngine;

namespace Klonamari
{
    public class CollectibleObjectSpawner : MonoBehaviour
    {
        public Transform attachRoot;
        public GameObject[] prefabs;

        public int toSpawn;

        public int minX, maxX, minZ, maxZ;

        void Start()
        {
            if (prefabs == null || prefabs.Length == 0)
            {
                return;
            }

            if (minX > maxX)
            {
                minX = maxX;
            }

            if (minZ > maxZ)
            {
                minZ = maxZ;
            }

            for (int i = 0; i < toSpawn; ++i)
            {
                int randomIndex = Random.Range(0, prefabs.Length);

                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                Vector3 randomPosition = new Vector3(randomX, 0, randomZ);

                GameObject created = Instantiate(prefabs[randomIndex], randomPosition, Quaternion.identity) as GameObject;
                created.transform.parent = attachRoot;
            }
        }
    }
}
