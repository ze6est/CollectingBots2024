using System;
using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class Ground : MonoBehaviour
    {
        public event Action<Vector3> Clicked;

        private void OnMouseDown()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                Clicked?.Invoke(hit.point);
        }
    }
}