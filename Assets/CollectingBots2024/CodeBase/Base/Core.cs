using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CollectingBots2024.CodeBase.Base
{
    public class Core : MonoBehaviour
    {
        [SerializeField] private ResourcesScaner _scaner;
        [SerializeField] private UnitSpawner _unitSpawner;
        
        [SerializeField] private List<Resource> _resources = new();
        [SerializeField] private Queue<NavMeshAgent> _unitsFree = new();
        [SerializeField] private List<NavMeshAgent> _unitsOccupied = new();

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

        private void OnSpawned(Unit unit)
        {
            if(unit.TryGetComponent(out NavMeshAgent agent))
            {
                _unitsFree.Enqueue(agent);
            }
        }

        public bool TryGetFreeUnit(out NavMeshAgent unit)
        {
            if (_unitsFree.Count > 0)
            {
                unit = _unitsFree.Dequeue();
                _unitsOccupied.Add(unit);

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
    }
}