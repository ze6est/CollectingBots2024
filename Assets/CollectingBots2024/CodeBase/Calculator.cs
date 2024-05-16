using System;
using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class Calculator
    {
        public Vector3[] GetCheckPoints(Vector3 spawnPosition, float resourceWidth)
        {
            Vector3[] checkPoints =
            {
                new Vector3(spawnPosition.x - resourceWidth / 2, spawnPosition.y, spawnPosition.z),
                new Vector3(spawnPosition.x + resourceWidth / 2, spawnPosition.y, spawnPosition.z),
                new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z - resourceWidth / 2),
                new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z + resourceWidth / 2),
            };

            return checkPoints;
        }
        
        public float GetWidth(GameObject @object)
        {
            Bounds bounds = GetBounds(@object);
            float width = Math.Max(bounds.max.x, bounds.max.z) * 2 ;

            return width;
        }

        public float GetSpawnHight(GameObject @object)
        {
            Bounds bounds = GetBounds(@object);
            float spawnHight = bounds.center.y + Mathf.Abs(bounds.min.y);

            return spawnHight;
        }

        private Bounds GetBounds(GameObject @object)
        {
            Renderer renderer = @object.GetComponent<Renderer>();
            Bounds bounds = renderer.bounds;

            return bounds;
        }
    }
}