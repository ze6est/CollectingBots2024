using CollectingBots2024.CodeBase.Base;
using UnityEngine;
using UnityEngine.AI;

namespace CollectingBots2024.CodeBase
{
    [RequireComponent(typeof(CapsuleCollider), typeof(NavMeshObstacle))]
    public class Resource : Target
    {
        public NavMeshObstacle Obstacle { get; private set; }
        public float Radius { get; private set; }

        private void Awake()
        {
            Obstacle = GetComponent<NavMeshObstacle>();
            Radius = GetComponent<CapsuleCollider>().radius;
        }

        private void OnEnable() => 
            Obstacle.enabled = true;
    }
}
