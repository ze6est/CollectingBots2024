using System.Collections;
using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class ResourceSpawner : Spawner<Resource>
    {
        [Header("Spawner settings:")]
        [SerializeField] private float _spawnInterval;

        private void Awake() => 
            Construct(AssetsPath.ResourcePrefabPath);

        private void OnEnable() => 
            StartCoroutine(SpawnResources());

        private IEnumerator SpawnResources()
        {
            WaitForSeconds wait = new WaitForSeconds(_spawnInterval);
            
            while (true)
            {
                yield return SpawnObject();
                yield return wait;
            }
        }
    }
}