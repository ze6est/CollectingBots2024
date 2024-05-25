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
        
        [Header("Resources pool Settings:")] 
        [SerializeField] private int _capacity = 10;
        [SerializeField] private bool _isAutoExpand = false;
        
        private ObjectsPool<T> _objectsPool;
        private T _prefab;
        private float _objectWidth;
        private float _objectPivotPositionY;
        
        private Calculator _calculator;

        public event Action<T> Spawned;

        protected void Construct(string prefabPath)
        {
            _prefab = Resources.Load<T>(prefabPath);
            
            _calculator = new Calculator();
            
            _objectsPool = new ObjectsPool<T>(_prefab, transform, _isAutoExpand, _capacity);

            _objectWidth = _calculator.GetWidth(_prefab.gameObject);
            _objectPivotPositionY = _calculator.GetSpawnHight(_prefab.gameObject);
        }

        public void Release(T @object) => 
            _objectsPool.Release(@object);

        protected IEnumerator SpawnObject()
        {
            float radius = _objectWidth / 2 + _spawnWidthOffset;
            float spawnHeight = _objectPivotPositionY + _spawnHightOffset;

            bool isSpawnPoint = false;
            
            while (isSpawnPoint == false)
            {
                Vector3 currentPosition = transform.position;
                float currentPositionX = currentPosition.x;
                float spawnPositionX = Random.Range(currentPositionX - _spawnRadius, currentPositionX + _spawnRadius);
                float currentPositionZ = currentPosition.z;
                float spawnPositionZ = Random.Range(currentPositionZ - _spawnRadius, currentPositionZ + _spawnRadius);
                
                Vector3 spawnPosition = new Vector3(spawnPositionX, spawnHeight, spawnPositionZ);
                
                isSpawnPoint = CheckPlatform(spawnPosition);

                if (isSpawnPoint)
                {
                    if (!Physics.CheckSphere(spawnPosition, radius, _collisionLayers))
                    {
                        T @object = _objectsPool.GetFreeObject();
                        @object.transform.position = spawnPosition;
                        Spawned?.Invoke(@object);
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

        private bool CheckPlatform(Vector3 spawnPosition)
        {
            Vector3[] checkPoints = _calculator.GetCheckPoints(spawnPosition, _objectWidth);

            foreach (Vector3 checkPoint in checkPoints)
            {
                RaycastHit[] results = new RaycastHit[1];

                if (Physics.RaycastNonAlloc(checkPoint, Vector3.down, results) == 0)
                    return false;
            }
            
            return true;
        }
    }
}