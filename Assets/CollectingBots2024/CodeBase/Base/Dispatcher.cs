using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace CollectingBots2024.CodeBase.Base
{
    public class Dispatcher : MonoBehaviour
    {
        [SerializeField] private Core _core;

        private NavMeshAgent _freeUnit;
        private Resource _resource;

        private Coroutine _sendUnitJob;

        private void OnEnable()
        {
            _sendUnitJob = StartCoroutine(SendUnit());
        }

        private void OnDisable()
        {
            StopCoroutine(_sendUnitJob);
        }

        private IEnumerator SendUnit()
        {
            while (true)
            {
                yield return FindFreeUnit();
                yield return FindResource();

                _freeUnit.stoppingDistance = _freeUnit.GetComponent<CapsuleCollider>().radius;

                Vector3 vector = _resource.transform.position - _freeUnit.transform.position;
                float distance = vector.magnitude;
                Vector3 direction = vector / distance;

                Vector3 destination = _resource.transform.position -
                                      direction * _resource.GetComponent<SphereCollider>().radius;
                
                _freeUnit.SetDestination(destination);
                Debug.Log("Destination");
            }
        }

        private IEnumerator FindFreeUnit()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            
            while (_core.TryGetFreeUnit(out _freeUnit) == false)
            {
                yield return wait;
            }
            
            Debug.Log("Find");
        }

        private IEnumerator FindResource()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            
            while (_core.TryGetResource(out _resource) == false)
            {
                yield return wait;
            }
        }
    }
}
