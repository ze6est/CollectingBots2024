using CollectingBots2024.CodeBase.Base;
using TMPro;
using UnityEngine;

namespace CollectingBots2024.CodeBase.UI
{
    public class ResourceCounterView : MonoBehaviour
    {
        [SerializeField] private ResourcesCounter _resourcesCounter;
        [SerializeField] private TextMeshProUGUI _counter;

        private void OnEnable() => 
            _resourcesCounter.CountChanged += OnCountChanged;

        private void OnDisable() => 
            _resourcesCounter.CountChanged -= OnCountChanged;

        private void OnCountChanged(int count) => 
            _counter.text = count.ToString();
    }
}