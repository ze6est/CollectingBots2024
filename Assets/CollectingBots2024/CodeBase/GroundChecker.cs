using UnityEngine;

namespace CollectingBots2024.CodeBase
{
    public class GroundChecker
    {
        private readonly Calculator _calculator;

        public GroundChecker(Calculator calculator) => 
            _calculator = calculator;

        public bool CheckGround(Vector3 spawnPosition, float objectRadius)
        {
            Vector3[] checkPoints = _calculator.GetCheckPoints(spawnPosition, objectRadius);

            foreach (Vector3 checkPoint in checkPoints)
            {
                RaycastHit[] results = new RaycastHit[1];

                if (Physics.RaycastNonAlloc(checkPoint, Vector3.down, results) == 0)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}