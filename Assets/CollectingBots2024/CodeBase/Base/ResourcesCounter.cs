using System;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(Core), typeof(UnitSpawner), typeof(Dispatcher))]
    public class ResourcesCounter : MonoBehaviour
    {
        private Core _core;
        private UnitSpawner _unitSpawner;
        private Dispatcher _dispatcher;

        private int _resourcesCount;

        public event Action<int> CountChanged;

        private void Awake()
        {
            _core = GetComponent<Core>();
            _unitSpawner = GetComponent<UnitSpawner>();
            _dispatcher = GetComponent<Dispatcher>();
        }

        private void OnEnable()
        {
            _core.ResourceDelivered += OnResourceDelivered;
            _unitSpawner.UnitSpawned += OnUnitSpawned;
            _dispatcher.UnitSentToFlag += OnUnitSentToFlag;
        }

        private void OnDisable()
        {
            _core.ResourceDelivered -= OnResourceDelivered;
            _unitSpawner.UnitSpawned -= OnUnitSpawned;
            _dispatcher.UnitSentToFlag -= OnUnitSentToFlag;
        }

        private void OnUnitSentToFlag(int costUnit, Units.Unit unit) => 
            _resourcesCount -= costUnit;

        private void OnResourceDelivered()
        {
            _resourcesCount++;
            CountChanged?.Invoke(_resourcesCount);
        }
        
        private void OnUnitSpawned(int costUnit)
        {
            _resourcesCount -= costUnit;
            CountChanged?.Invoke(_resourcesCount);
        }
    }
}