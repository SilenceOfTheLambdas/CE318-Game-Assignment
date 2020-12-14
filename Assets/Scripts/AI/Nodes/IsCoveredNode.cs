using UnityEngine;

namespace AI.LeafNodes
{
    public class IsCoveredNode : BTNode
    {
        private Transform target;
        private Transform origin;

        public IsCoveredNode(BehaviourTree t, Transform target, Transform origin) : base(t)
        {
            this.target = target;
            this.origin = origin;
        }

        public override Result Execute()
        {
            if (Physics.Raycast(origin.position, target.position - origin.position, out var hit))
            {
                if (hit.collider.transform != target)
                {
                    return Result.Success;
                }
            }
            return Result.Failure;
        }
    }
}