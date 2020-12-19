using UnityEngine;

namespace AI.Nodes
{
    public class NotInRangeNode : BTNode
    {
        private readonly float     _range;
        private readonly Transform _target;
        private readonly Transform _origin;

        public NotInRangeNode(float range, Transform target, Transform origin)
        {
            _range = range;
            _target = target;
            _origin = origin;
        }

        public override Result Execute()
        {
            var distance = Vector3.Distance(_target.position, _origin.position);
            return distance <= _range ? Result.Failure : Result.Success;
        }
    }
}