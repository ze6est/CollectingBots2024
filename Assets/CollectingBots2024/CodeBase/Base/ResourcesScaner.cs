using System;
using System.Collections;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    public class ResourcesScaner : MonoBehaviour
    {
        [SerializeField] private LayerMask _collisionLayers;
        [Range(0, 31)]
        [SerializeField] private int _resourcesFoundLayer;
        [SerializeField] private float _scanRadius;
        [SerializeField] private float _scanInterval;
        [SerializeField] private int _resourcesFoundPerScan;
        
        private Coroutine _scanJob;

        public event Action<Collider> ResourceFound; 

        private void OnEnable() => 
            _scanJob = StartCoroutine(Scan());

        private void OnDisable() => 
            StopCoroutine(_scanJob);

        private IEnumerator Scan()
        {
            WaitForSeconds wait = new WaitForSeconds(_scanInterval);
            
            while (true)
            {
                Collider[] resources = new Collider[_resourcesFoundPerScan];

                int count = Physics.OverlapSphereNonAlloc(transform.position, _scanRadius, resources, _collisionLayers);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        ResourceFound?.Invoke(resources[i]);
                        resources[i].gameObject.layer = _resourcesFoundLayer;
                    }
                }
                
                yield return wait;
            }
        }
    }
}