using UnityEngine;

namespace AI.Nodes
{
    public class IsCoveredNode : BTNode
    {
        private readonly Transform _target;
        private readonly Transform _origin;

        public IsCoveredNode(Transform target, Transform origin)
        {
            _target = target;
            _origin = origin;
        }

        public override Result Execute()
        {
            if (Physics.Raycast(_origin.position, _target.position - _origin.position, out var hit))
            {
                if (hit.collider.transform != _target)
                {
                    return Result.Success;
                }
            }
            return Result.Failure;
        }
    }
}