using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CollectingBots2024.CodeBase.Spawners
{
    public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private LayerMask _collisionLayers;
        [SerializeField] private float _spawnRadius;
        [SerializeField] private float _spawnWidthOffset;
        [SerializeField] private float _spawnHightOffset;
        [SerializeField] private float _defaultObjectRadius = 1f;
        
        [Header("Pool Settings:")] 
        [SerializeField] private int _capacity = 10;
        [SerializeField] private bool _isAutoExpand = false;
        
        private ObjectsPool<T> _objectsPool;
        private GroundChecker _groundChecker;

        private float _objectRadius;

        public event Action<T> Spawned;
        public event Action<T> Removed;

        protected int Capacity => _capacity;
        
        public virtual void Construct(T prefab, GroundChecker groundChecker, bool isStartSpawn = true)
        {
            _groundChecker = groundChecker;
            
            _objectRadius = prefab.TryGetComponent(out CapsuleCollider capsuleCollider) ? capsuleCollider.radius : _defaultObjectRadius;
            
            _objectsPool = new ObjectsPool<T>(prefab, transform, _isAutoExpand, _capacity);
        }

        public void Add(T @object)
        {
            _objectsPool.Add(@object);
            Spawned?.Invoke(@object);
        }

        public void Remove(T @object)
        {
            _objectsPool.Remove(@object);
            Removed?.Invoke(@object);
        }

        public void Release(T @object) => 
            _objectsPool.Release(@object);

        protected IEnumerator SpawnObject()
        {
            float radius = _objectRadius + _spawnWidthOffset;

            bool isSpawnPoint = false;
            
            while (isSpawnPoint == false)
            {
                Vector3 currentPosition = transform.position;
                float currentPositionX = currentPosition.x;
                float spawnPositionX = Random.Range(currentPositionX - _spawnRadius, currentPositionX + _spawnRadius);
                float currentPositionZ = currentPosition.z;
                float spawnPositionZ = Random.Range(currentPositionZ - _spawnRadius, currentPositionZ + _spawnRadius);
                
                Vector3 spawnPosition = new Vector3(spawnPositionX, _spawnHightOffset, spawnPositionZ);

                isSpawnPoint = _groundChecker.CheckGround(spawnPosition, _objectRadius);

                if (isSpawnPoint)
                {
                    if (!Physics.CheckSphere(spawnPosition, radius, _collisionLayers))
                    {
                        T @object = _objectsPool.GetFreeObject();

                        if (@object != null)
                        {
                            @object.transform.position = spawnPosition;
                            Spawned?.Invoke(@object);
                        }
                    }
                    else
                    {
                        isSpawnPoint = false;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}