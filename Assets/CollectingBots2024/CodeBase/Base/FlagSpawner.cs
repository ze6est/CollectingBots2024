using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(Core))]
    public class FlagSpawner : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private LayerMask _collisionLayers;
        
        private Ground _ground;
        private Flag _flagPrefab;
        private Flag _flag;
        private Core _core;
        private GroundChecker _groundChecker;
        
        private bool _isFlagReadyToInstalled;
        private bool _isFlagSpawned;

        public event Action<Flag> FlagSpawned;
        
        public void Construct(Ground ground, GroundChecker groundChecker)
        {
            _ground = ground;
            _groundChecker = groundChecker;
            _ground.Clicked += OnClicked;
        }

        private void Awake()
        {
            _flagPrefab = Resources.Load<Flag>(AssetsPath.FlagPrefabPath);
            _core = GetComponent<Core>();
            _isFlagReadyToInstalled = false;
            _isFlagSpawned = false;
        }

        private void OnDisable() => 
            _ground.Clicked -= OnClicked;

        private void OnClicked(Vector3 position)
        {
            TrySpawnFlag(position);

            if (_flag != null)
            {
                if (CheckPosition(position))
                {
                    _flag.transform.position = position;
                }
            }
        }

        private bool TrySpawnFlag(Vector3 position)
        {
            if (_isFlagReadyToInstalled && _isFlagSpawned == false && CheckPosition(position))
            {
                _flag = Instantiate(_flagPrefab, position, Quaternion.identity);
                _isFlagSpawned = true;
                _isFlagReadyToInstalled = false;
                FlagSpawned?.Invoke(_flag);
                
                return true;
            }
            
            return false;
        }

        private bool CheckPosition(Vector3 position)
        {
            bool isSpawnPoint = _groundChecker.CheckGround(position + Vector3.up, _core.Radius);
            bool isPositionFree = !Physics.CheckSphere(position, _core.Radius, _collisionLayers);

            if (isSpawnPoint && isPositionFree)
                return true;

            return false;
        }

        public void OnPointerClick(PointerEventData eventData) => 
            _isFlagReadyToInstalled = true;
    }
}