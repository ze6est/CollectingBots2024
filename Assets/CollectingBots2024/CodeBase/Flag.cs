using CollectingBots2024.CodeBase.Base;
using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class Flag : Target
    {
        [SerializeField] private float _radius = 2;

        public float Radius => _radius;
    }
}
