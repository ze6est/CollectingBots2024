using System;
using System.Collections;
using CollectingBots2024.CodeBase.Spawners;
using CollectingBots2024.CodeBase.Units;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(ResourcesCounter))]
    public class UnitSpawner : Spawner<Unit>
    {
        [Header("Spawner settings:")] 
        [SerializeField] private int _startUnitsCount = 3;

        [SerializeField] private int _maxCountUnit = 5;
        [SerializeField] private int _costUnit = 3;
        [SerializeField] private float _delay = 0.1f;

        private ResourcesCounter _resourcesCounter;

        private int _countUnits;

        public event Action<int> UnitSpawned;
        public event Action MaxCountUnitSpawned;

        public override void Construct(Unit prefab, GroundChecker groundChecker, bool isStartSpawn)
        {
            base.Construct(prefab, groundChecker, isStartSpawn);
            
            if(isStartSpawn)
                StartCoroutine(SpawnStartUnits(_startUnitsCount));
        }

        private void Awake() => 
            _resourcesCounter = GetComponent<ResourcesCounter>();

        private void OnEnable() => 
            _resourcesCounter.CountChanged += OnResourceDelivered;

        private void OnDisable() => 
            _resourcesCounter.CountChanged -= OnResourceDelivered;

        private void OnResourceDelivered(int countResources)
        {
            if (countResources >= _costUnit && _countUnits < Capacity)
            {
                StartCoroutine(SpawnObject());
                _countUnits++;
                UnitSpawned?.Invoke(_costUnit);

                if (_countUnits >= _maxCountUnit)
                {
                    MaxCountUnitSpawned?.Invoke();
                }
            }
        }

        private IEnumerator SpawnStartUnits(int count)
        {
            WaitForSeconds wait = new WaitForSeconds(_delay);

            for (int i = 0; i < count; i++)
            {
                yield return SpawnObject();
                _countUnits++;
                yield return wait;
            }
        }
    }
}