using System.Collections;
using CollectingBots2024.CodeBase.Spawners;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    public class UnitSpawner : Spawner<Unit>
    {
        [Header("Spawner settings:")]
        [SerializeField] private int _statrUnitsCount = 3;
        
        private void Awake() => 
            Construct(AssetsPath.UnitPrefabPath);

        private void Start() => 
            StartCoroutine(SpawnStartUnits(_statrUnitsCount));

        private IEnumerator SpawnStartUnits(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.1f);
                yield return SpawnObject();
            }
        }
    }
}