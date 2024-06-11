using System;
using System.Collections;
using CollectingBots2024.CodeBase.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace CollectingBots2024.CodeBase.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Transform _transferPoint;
        [Range(0, 31)]
        [SerializeField] private int _resourcesCollectedLayer;
        [SerializeField] private float _offsetToTarget = 0.15f;

        [SerializeField] private float _jumpPower = 1f;
        [SerializeField] private int _numsJump = 1;
        [SerializeField] private float _durationJump = 0.5f;
        
        private NavMeshAgent _agent;
        private Target _target;
        private Resource _resource;

        private float _checkTime = 0.5f;
        private Vector3 _currentTargetPosition;
        private float _currentOffset;
        
        private Coroutine _checkTargetDestinationJob;

        public event Action<Unit, Resource> ResourceCollected;
        public event Action<Unit, Resource> ResourceDelivered;
        public event Action<Vector3> FlagDestroyed;
        
        public float Radius { get; private set; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            Radius = GetComponent<CapsuleCollider>().radius;
        }

        public void SetDestination(Target target, float offsetToTarget)
        {
            _target = target;
            _currentTargetPosition = target.transform.position;
            _currentOffset = offsetToTarget;
            
            _agent.SetDestination(CalculateDestination(target, offsetToTarget));
            
            _checkTargetDestinationJob = StartCoroutine(CheckTargetDestination());
        }

        private IEnumerator CheckTargetDestination()
        {
            WaitForSeconds wait = new WaitForSeconds(_checkTime);
            yield return null;

            while (_agent.remainingDistance >= _agent.stoppingDistance + _offsetToTarget)
            {
                if (_currentTargetPosition != _target.transform.position)
                {
                    _agent.SetDestination(CalculateDestination(_target, _currentOffset));
                }
                
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
                _resource.transform.DOJump(_transferPoint.position, _jumpPower, _numsJump, _durationJump).
                    OnComplete(() =>
                        {
                            _resource.transform.position = _transferPoint.position;
                            _resource.transform.parent = gameObject.transform;
                            
                            StopCoroutine(_checkTargetDestinationJob);
                            ResourceCollected?.Invoke(this, _resource);
                        });
            }
            else if (_target.TryGetComponent(out Flag flag))
            {
                Vector3 position = flag.transform.position;
                Destroy(flag.gameObject);
                
                FlagDestroyed?.Invoke(position);
            }

            _target = null;
        }

        private Vector3 CalculateDestination(Target target, float offsetToTarget)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            Vector3 vector = targetPosition - gameObject.transform.position;
            float distance = vector.magnitude;
            Vector3 direction = vector / distance;
            Vector3 destination = targetPosition - direction * offsetToTarget;

            return destination;
        }
    }
}