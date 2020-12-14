using UnityEngine;

namespace AI.LeafNodes
{
    public class RangeNode : BTNode
    {
        private float     range;
        private Transform target;
        private Transform origin;

        public RangeNode(BehaviourTree t, float range, Transform target, Transform origin) : base(t)
        {
            this.range = range;
            this.target = target;
            this.origin = origin;
        }

        public override Result Execute()
        {
            var distance = Vector3.Distance(target.position, origin.position);
            return distance <= range ? Result.Success : Result.Failure;
        }
    }
}