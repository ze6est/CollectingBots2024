using System.Collections;
using CollectingBots2024.CodeBase.Spawners;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    public class Dispatcher : MonoBehaviour
    {
        [SerializeField] private ResourceSpawner _resourceSpawner;
        [SerializeField] private Core _core;
        [Range(0, 31)]
        [SerializeField] private int _resourcesSpawnedLayer;
        [SerializeField] private float _minOffsetToTarget = 0.15f;
        [SerializeField] private float _searchDelay = 1f;

        private Unit _freeUnit;
        private Resource _resource;

        private Coroutine _sendUnitJob;

        private void OnEnable() => 
            _sendUnitJob = StartCoroutine(SendUnit());

        private void OnDisable() => 
            StopCoroutine(_sendUnitJob);

        private IEnumerator SendUnit()
        {
            while (true)
            {
                yield return FindFreeUnit();
                yield return FindResource();

                _freeUnit.ResourceCollected += OnResourceCollected;
                _freeUnit.SetDestination(_resource.gameObject, _resource.Radius + _freeUnit.Radius + _minOffsetToTarget);
            }
        }

        private void OnResourceCollected(Unit unit, Resource resource)
        {
            unit.ResourceDelivered += OnResourceDelivered;
            unit.SetDestination(gameObject, _core.Radius + resource.Radius * 2 + unit.Radius + _minOffsetToTarget);
            
            unit.ResourceCollected -= OnResourceCollected;
        }

        private void OnResourceDelivered(Unit unit, Resource resource)
        {
            resource.transform.parent = _resourceSpawner.transform;
            _resourceSpawner.Release(resource);
            resource.gameObject.layer = _resourcesSpawnedLayer;
            
            unit.ResourceDelivered -= OnResourceDelivered;
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
