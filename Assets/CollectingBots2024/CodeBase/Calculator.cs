using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class Calculator
    {
        public Vector3[] GetCheckPoints(Vector3 spawnPosition, float resourceRadius)
        {
            Vector3[] checkPoints =
            {
                new(spawnPosition.x - resourceRadius, spawnPosition.y, spawnPosition.z),
                new(spawnPosition.x + resourceRadius, spawnPosition.y, spawnPosition.z),
                new(spawnPosition.x, spawnPosition.y, spawnPosition.z - resourceRadius),
                new(spawnPosition.x, spawnPosition.y, spawnPosition.z + resourceRadius),
            };

            return checkPoints;
        }
    }
}