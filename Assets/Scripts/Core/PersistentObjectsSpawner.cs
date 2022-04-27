using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject persistentObjectsPrefab;

        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned) return;

            SpawnPersistentObjects();

            hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObjects = Instantiate(persistentObjectsPrefab);
            DontDestroyOnLoad(persistentObjects);
        }
    }
}
