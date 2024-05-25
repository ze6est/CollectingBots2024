using System;
using System.Collections;
using CollectingBots2024.CodeBase.Base;
using UnityEngine;
using UnityEngine.AI;

namespace CollectingBots2024.CodeBase
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Unit : MonoBehaviour
    {
        [Range(0, 31)]
        [SerializeField] private int _resourcesCollectedLayer;
        [SerializeField] private float _liftingHeightResource = 0.5f;
        [SerializeField] private float _offsetToTarget = 0.15f;
        
        private NavMeshAgent _agent;
        private GameObject _target;
        private Resource _resource;

        private float _checkTime = 0.5f;

        private Coroutine _checkTargetDestinationJob;

        public event Action<Unit, Resource> ResourceCollected;
        public event Action<Unit, Resource> ResourceDelivered;
        
        public float Radius { get; private set; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            Radius = GetComponent<CapsuleCollider>().radius;
        }

        public void SetDestination(GameObject target, float offsetToTarget)
        {
            _target = target;
            
            Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            Vector3 vector = targetPosition - gameObject.transform.position;
            float distance = vector.magnitude;
            Vector3 direction = vector / distance;
            Vector3 destination = targetPosition - direction * offsetToTarget;

            _agent.SetDestination(destination);

            _checkTargetDestinationJob = StartCoroutine(CheckTargetDestination());
        }

        private IEnumerator CheckTargetDestination()
        {
            WaitForSeconds wait = new WaitForSeconds(_checkTime);
            yield return null;

            while (_agent.remainingDistance >= _agent.stoppingDistance + _offsetToTarget)
            {
                yield return wait;
            }

            if (_target.TryGetComponent(out Dispatcher _))
            {
                StopCoroutine(_checkTargetDestinationJob);
                ResourceDelivered?.Invoke(this, _resource);
            }
            else if (_target.TryGetComponent(out _resource))
            {
                _resource.Obstacle.enabled = false;
                _resource.gameObject.layer = _resourcesCollectedLayer;
                _resource.transform.position += Vector3.up * _liftingHeightResource;
                _resource.transform.parent = gameObject.transform;
                
                StopCoroutine(_checkTargetDestinationJob);
                ResourceCollected?.Invoke(this, _resource);
            }
        }
    }
}