using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    public class ResourcesScaner : MonoBehaviour
    {
        [SerializeField] private LayerMask _collisionLayers;
        [Range(0, 31)]
        [SerializeField] private int _resourcesFoundNumber;
        [SerializeField] private float _scanRadius;
        [SerializeField] private float _scanInterval;
        [SerializeField] private int _resourcesFoundPerScan;
        
        [SerializeField] private List<Collider> _resources = new List<Collider>();
        private Coroutine _scanJob;

        private void OnEnable() => 
            _scanJob = StartCoroutine(Scan());

        private void OnDisable() => 
            StopCoroutine(_scanJob);

        private IEnumerator Scan()
        {
            var wait = new WaitForSeconds(_scanInterval);
            
            while (true)
            {
                Collider[] resources = new Collider[_resourcesFoundPerScan];

                int count = Physics.OverlapSphereNonAlloc(transform.position, _scanRadius, resources, _collisionLayers);

                for (int i = 0; i < count; i++)
                {
                    _resources.Add(resources[i]);
                    resources[i].gameObject.layer = _resourcesFoundNumber;
                }
                
                yield return wait;
            }
        }
    }
}