using UnityEngine;
using UnityEngine.AI;

namespace CollectingBots2024.CodeBase
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(NavMeshObstacle))]
    public class Resource : MonoBehaviour
    {
        public NavMeshObstacle Obstacle { get; private set; }
        public float Radius { get; private set; }

        private void Awake()
        {
            Obstacle = GetComponent<NavMeshObstacle>();
            Radius = GetComponent<SphereCollider>().radius;
        }

        private void OnEnable() => 
            Obstacle.enabled = true;
    }
}
