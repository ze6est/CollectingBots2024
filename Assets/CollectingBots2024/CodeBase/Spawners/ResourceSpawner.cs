using System.Collections;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Spawners
{
    public class ResourceSpawner : Spawner<Resource>
    {
        [Header("Spawner settings:")]
        [SerializeField] private float _spawnInterval;

        public override void Construct(Resource prefab, GroundChecker groundChecker, bool isStartSpawn)
        {
            base.Construct(prefab, groundChecker);
            
            if(isStartSpawn)
                StartCoroutine(SpawnResources());
        }

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