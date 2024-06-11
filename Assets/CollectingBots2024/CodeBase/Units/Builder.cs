using CollectingBots2024.CodeBase.Base;
using CollectingBots2024.CodeBase.Spawners;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Units
{
    [RequireComponent(typeof(Unit))]
    public class Builder : MonoBehaviour
    {
        private Core _corePrefab;
        private Unit _unit;

        private Unit _unitPrefab;
        private ResourceSpawner _resourceSpawner;
        private Ground _ground;
        private GroundChecker _groundChecker;

        public void Construct(Unit unitPrefab, ResourceSpawner resourceSpawner, Ground ground, GroundChecker groundChecker)
        {
            _unitPrefab = unitPrefab;
            _resourceSpawner = resourceSpawner;
            _ground = ground;
            _groundChecker = groundChecker;
        }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _corePrefab = Resources.Load<Core>(AssetsPath.CorePrefabPath);
        }

        private void OnEnable() => 
            _unit.FlagDestroyed += OnFlagDestroyed;

        private void OnDisable() => 
            _unit.FlagDestroyed -= OnFlagDestroyed;

        private void OnFlagDestroyed(Vector3 position)
        {
            Core core = Instantiate(_corePrefab, position, Quaternion.identity);

            if(core.TryGetComponent(out Dispatcher dispatcher))
                dispatcher.Construct(_unitPrefab, _resourceSpawner, _ground, _groundChecker);
            
            if(core.TryGetComponent(out FlagSpawner flagSpawner))
                flagSpawner.Construct(_ground, _groundChecker);
            
            if (core.TryGetComponent(out UnitSpawner unitSpawner))
            {
                unitSpawner.Construct(_unitPrefab, _groundChecker, false);
                unitSpawner.Add(_unit);
                _unit.transform.parent = unitSpawner.transform;
            }
        }
    }
}
