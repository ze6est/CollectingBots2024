using System;
using System.Collections.Generic;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Core : MonoBehaviour
    {
        [SerializeField] private ResourcesScaner _scaner;
        [SerializeField] private UnitSpawner _unitSpawner;
        
        private List<Resource> _resources = new();
        private Queue<Unit> _unitsFree = new();

        public event Action ResourceDelivered;
        
        public float Radius { get; private set; }

        private void Awake() => 
            Radius = GetComponent<CapsuleCollider>().radius;

        private void OnEnable()
        {
            _scaner.ResourceFound += OnResourceFound;
            _unitSpawner.Spawned += OnSpawned;
        }

        private void OnDisable()
        {
            _scaner.ResourceFound -= OnResourceFound;
            _unitSpawner.Spawned -= OnSpawned;
        }

        private void OnResourceFound(Collider collider)
        {
            if(collider.TryGetComponent(out Resource resource))
                _resources.Add(resource);
        }

        private void OnSpawned(Unit unit) => 
            _unitsFree.Enqueue(unit);

        public bool TryGetFreeUnit(out Unit unit)
        {
            if (_unitsFree.Count > 0)
            {
                unit = _unitsFree.Dequeue();
                unit.ResourceDelivered += OnResourceDelivered;

                return true;
            }

            unit = null;
            
            return false;
        }

        private void OnResourceDelivered(Unit unit, Resource _)
        {
            ResourceDelivered?.Invoke();
            unit.ResourceDelivered -= OnResourceDelivered;
            _unitsFree.Enqueue(unit);
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
    }
}