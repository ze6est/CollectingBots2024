using System;
using System.Collections.Generic;
using CollectingBots2024.CodeBase.Units;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(BoxCollider), typeof(FlagSpawner), typeof(Dispatcher))]
    [RequireComponent(typeof(ResourcesScaner), typeof(UnitSpawner))]
    public class Core : MonoBehaviour
    {
        private ResourcesScaner _scaner;
        private UnitSpawner _unitSpawner;
        private FlagSpawner _flagSpawner;
        private Dispatcher _dispatcher;
        private List<Resource> _resources = new();
        private List<Unit> _unitsFree = new();

        public event Action ResourceDelivered;
        
        public float Radius { get; private set; }

        private void Awake()
        {
            _scaner = GetComponent<ResourcesScaner>();
            _unitSpawner = GetComponent<UnitSpawner>();
            _flagSpawner = GetComponent<FlagSpawner>();
            _dispatcher = GetComponent<Dispatcher>();
            Radius = GetComponent<BoxCollider>().size.x / 2;
        }

        private void OnEnable()
        {
            _scaner.ResourceFound += OnResourceFound;
            _unitSpawner.Spawned += OnSpawned;
            _unitSpawner.MaxCountUnitSpawned += OnMaxCountUnitSpawned;
            _flagSpawner.FlagSpawned += OnFlagSpawned;
            _dispatcher.UnitSentToFlag += OnUnitSentToFlag;
            _unitSpawner.Removed += OnRemoved;
        }

        private void OnDisable()
        {
            _scaner.ResourceFound -= OnResourceFound;
            _unitSpawner.Spawned -= OnSpawned;
            _unitSpawner.MaxCountUnitSpawned -= OnMaxCountUnitSpawned;
            _flagSpawner.FlagSpawned -= OnFlagSpawned;
            _dispatcher.UnitSentToFlag -= OnUnitSentToFlag;
            _unitSpawner.Removed -= OnRemoved;
        }

        public bool TryGetFreeUnit(out Unit unit)
        {
            if (_unitsFree.Count > 0)
            {
                unit = _unitsFree[0];
                _unitsFree.Remove(unit);
                unit.ResourceDelivered += OnResourceDelivered;

                return true;
            }

            unit = null;
            
            return false;
        }
        
        public bool TryGetResource(out Resource resource)
        {
            if (_resources.Count > 0)
            {
                resource = _resources[0];
                _resources.Remove(resource);

                return true;
            }
            
            resource = null;

            return false;
        }

        private void OnResourceFound(Collider collider)
        {
            if(collider.TryGetComponent(out Resource resource))
                _resources.Add(resource);
        }

        private void OnSpawned(Unit unit) => 
            _unitsFree.Add(unit);

        private void OnResourceDelivered(Unit unit, Resource _)
        {
            ResourceDelivered?.Invoke();
            unit.ResourceDelivered -= OnResourceDelivered;
            _unitsFree.Add(unit);
        }

        private void OnMaxCountUnitSpawned() => 
            _unitSpawner.enabled = false;
        
        private void OnFlagSpawned(Flag flag) => 
            _unitSpawner.enabled = false;

        private void OnUnitSentToFlag(int cost, Unit unit) => 
            _unitSpawner.enabled = true;

        private void OnRemoved(Unit unit)
        {
            unit.ResourceDelivered -= OnResourceDelivered;
            _unitsFree.Remove(unit);
        }
    }
}