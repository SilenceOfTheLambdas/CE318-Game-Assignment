using Enemies;
using UnityEngine;

namespace AI.LeafNodes
{
    public class IsCoverAvailableNode : BTNode
    {
        private Cover[]   _availableCovers;
        private Transform _target;
        private EnemyAI   _enemyAI;

        public IsCoverAvailableNode(BehaviourTree t, Cover[] availableCovers, Transform target, EnemyAI enemyAI) : base(t)
        {
            _availableCovers = availableCovers;
            _target = target;
            _enemyAI = enemyAI;
        }

        public override Result Execute()
        {
            var bestSpot = FindBestCoverSpot();
            _enemyAI.SetBestCoverSpot(bestSpot);
            return bestSpot != null ? Result.Success : Result.Failure;
        }

        private Transform FindBestCoverSpot()
        {
            float     minAngle = 90;
            Transform bestSpot = null;

            for (int i = 0; i < _availableCovers.Length; i++)
            {
                Transform bestSpotInCover = FindBestSpotInCover(_availableCovers[i], ref minAngle);
                if (bestSpotInCover != null)
                {
                    bestSpot = bestSpotInCover;
                }
            }

            return bestSpot;
        }

        private Transform FindBestSpotInCover(Cover cover, ref float minAngle)
        {
            Transform[] availableSpots = cover.GetCoverSpots();
            Transform   bestSpot       = null;
            for (int i = 0; i < _availableCovers.Length; i++)
            {
                var direction = _target.position - availableSpots[i].position;
                if (CheckIfSpotIsValid(_availableCovers[i].transform))
                {
                    var angle = Vector3.Angle(availableSpots[i].forward, direction);
                    if (angle < minAngle)
                    {
                        minAngle = angle;
                        bestSpot = availableSpots[i];
                    }
                }
            }

            return bestSpot;
        }

        private bool CheckIfSpotIsValid(Transform spot)
        {
            RaycastHit hit;
            Vector3    direction = _target.position - spot.position;
            if (Physics.Raycast(spot.position, direction, out hit))
            {
                if (hit.collider.transform != _target)
                {
                    return true;
                }
            }

            return false;
        }
    }
}