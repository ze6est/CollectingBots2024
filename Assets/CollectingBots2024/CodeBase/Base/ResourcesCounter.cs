using System;
using UnityEngine;

namespace CollectingBots2024.CodeBase.Base
{
    [RequireComponent(typeof(Core))]
    public class ResourcesCounter : MonoBehaviour
    {
        private Core _core;
        
        private int _resourcesCount;

        public event Action<int> ResourceDelivered;

        private void Awake()
        {
            _core = GetComponent<Core>();
        }

        private void OnEnable() => 
            _core.ResourceDelivered += OnResourceDelivered;

        private void OnDisable() => 
            _core.ResourceDelivered -= OnResourceDelivered;

        private void OnResourceDelivered()
        {
            _resourcesCount++;
            ResourceDelivered?.Invoke(_resourcesCount);
        }
    }
}