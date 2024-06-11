using CollectingBots2024.CodeBase.Base;
using CollectingBots2024.CodeBase.Spawners;
using CollectingBots2024.CodeBase.Units;
using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private Resource _resourcePrefab;
        [SerializeField] private Ground _groundPrefab;
        [SerializeField] private Core _corePrefab;
        [SerializeField] private ResourceSpawner _resourcesSpawnerPrefab;
        
        private Ground _ground;
        private Core _core;
        private ResourceSpawner _resourcesSpawner;

        private GroundChecker _groundChecker;
        private Calculator _calculator;


        private void Awake()
        {
            InstantiateWorld();
            Init();
        }

        private void InstantiateWorld()
        {
            _calculator = new Calculator();
            _groundChecker = new GroundChecker(_calculator);
            
            _ground = Instantiate(_groundPrefab);
            _core = Instantiate(_corePrefab, _startPoint);
            _resourcesSpawner = Instantiate(_resourcesSpawnerPrefab);
        }

        private void Init()
        {
            if(_core.TryGetComponent(out Dispatcher dispatcher))
                dispatcher.Construct(_unitPrefab, _resourcesSpawner, _ground, _groundChecker);
            
            if(_core.TryGetComponent(out FlagSpawner flagSpawner))
                flagSpawner.Construct(_ground, _groundChecker);
            
            if(_core.TryGetComponent(out UnitSpawner unitSpawner))
                unitSpawner.Construct(_unitPrefab, _groundChecker, true);
            
            _resourcesSpawner.Construct(_resourcePrefab, _groundChecker, true);
        }
    }
}
