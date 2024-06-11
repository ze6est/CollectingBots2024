using System;
using System.Collections;
using CollectingBots2024.CodeBase.Spawners;
using CollectingBots2024.CodeBase.Units;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(FlagSpawner), typeof(ResourcesCounter), typeof(UnitSpawner))]
    [RequireComponent(typeof(Core))]
    public class Dispatcher : Target
    {
        [Range(0, 31)]
        [SerializeField] private int _resourcesSpawnedLayer;
        [SerializeField] private float _minOffsetToTarget = 0.15f;
        [SerializeField] private float _searchDelay = 1f;
        [SerializeField] private int _sendToFlagCost = 5;
        
        private Core _core;
        private FlagSpawner _flagSpawner;
        private Unit _freeUnit;
        private Resource _resource;
        private ResourcesCounter _counter;
        private Flag _flag;
        private UnitSpawner _unitSpawner;
        
        private Unit _unitPrefab;
        private ResourceSpawner _resourceSpawner;
        private Ground _ground;
        private GroundChecker _groundChecker;

        private Coroutine _sendUnitToResourceJob;

        private bool _isUnitSentToFlag;

        public event Action<int, Unit> UnitSentToFlag;
        
        public void Construct(Unit unitPrefab, ResourceSpawner resourceSpawner, Ground ground, GroundChecker groundChecker)
        {
            _unitPrefab = unitPrefab;
            _resourceSpawner = resourceSpawner;
            _ground = ground;
            _groundChecker = groundChecker;
            
            _sendUnitToResourceJob = StartCoroutine(SendUnitToResource());
        }

        private void Awake()
        {
            _core = GetComponent<Core>();
            _flagSpawner = GetComponent<FlagSpawner>();
            _counter = GetComponent<ResourcesCounter>();
            _unitSpawner = GetComponent<UnitSpawner>();
        }

        private void OnEnable()
        {
            _flagSpawner.FlagSpawned += OnFlagSpawned;
            _unitSpawner.Spawned += OnUnitSpawned;
        }

        private void OnDisable()
        {
            StopCoroutine(_sendUnitToResourceJob);
            _flagSpawner.FlagSpawned -= OnFlagSpawned;
            _unitSpawner.Spawned -= OnUnitSpawned;
        }

        private IEnumerator SendUnitToResource()
        {
            while (true)
            {
                yield return FindFreeUnit();
                yield return FindResource();

                _freeUnit.ResourceCollected += OnResourceCollected;
                _freeUnit.SetDestination(_resource, _resource.Radius + _freeUnit.Radius + _minOffsetToTarget);
            }
        }

        private void OnResourceCollected(Unit unit, Resource resource)
        {
            unit.ResourceDelivered += OnResourceDelivered;
            unit.SetDestination(this, _core.Radius + resource.Radius * 2 + unit.Radius + _minOffsetToTarget);
            
            unit.ResourceCollected -= OnResourceCollected;
        }

        private void OnResourceDelivered(Unit unit, Resource resource)
        {
            resource.transform.parent = _resourceSpawner.transform;
            _resourceSpawner.Release(resource);
            resource.gameObject.layer = _resourcesSpawnedLayer;
            
            unit.ResourceDelivered -= OnResourceDelivered;
        }
        
        private void OnFlagSpawned(Flag flag)
        {
            _flag = flag;
            _counter.CountChanged += OnCountChanged;
        }

        private void OnCountChanged(int count)
        {
            if (count >= _sendToFlagCost)
            {
                StopCoroutine(_sendUnitToResourceJob);
                StartCoroutine(SendUnitToFlag(_flag));
                _counter.CountChanged -= OnCountChanged;
            }
        }
        
        private void OnUnitSpawned(Unit unit)
        {
            if(unit.TryGetComponent(out Builder builder))
                builder.Construct(_unitPrefab, _resourceSpawner, _ground, _groundChecker);
        }

        private IEnumerator SendUnitToFlag(Flag flag)
        {
            while (_isUnitSentToFlag == false)
            {
                yield return FindFreeUnit();
                
                _freeUnit.SetDestination(flag, flag.Radius + _freeUnit.Radius + _minOffsetToTarget);
                
                _freeUnit.ResourceCollected -= OnResourceCollected;
                _freeUnit.ResourceDelivered -= OnResourceDelivered;
                
                _isUnitSentToFlag = true;
                _unitSpawner.Remove(_freeUnit);
                _freeUnit.transform.parent = null;
                UnitSentToFlag?.Invoke(_sendToFlagCost, _freeUnit);
                _sendUnitToResourceJob = StartCoroutine(SendUnitToResource());
            }
        }

        private IEnumerator FindFreeUnit()
        {
            WaitForSeconds wait = new WaitForSeconds(_searchDelay);
            
            while (_core.TryGetFreeUnit(out _freeUnit) == false)
                yield return wait;
        }

        private IEnumerator FindResource()
        {
            WaitForSeconds wait = new WaitForSeconds(_searchDelay);
            
            while (_core.TryGetResource(out _resource) == false)
                yield return wait;
        }
    }
}
