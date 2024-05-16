using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CollectingBots2024.CodeBase
{
    public class UnitSpawner : Spawner<Unit>
    {
        [Header("Settings:")]
        [SerializeField] private int _statrUnitsCount = 3;
        
        private void Awake()
        {
            Construct(AssetsPath.UnitPrefabPath);
        }

        private void Start()
        {
            StartCoroutine(SpawnStartUnits(_statrUnitsCount));
        }

        protected override IEnumerator SpawnObjects()
        {
            yield return new WaitForSeconds(60);
        }

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