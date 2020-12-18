using UnityEngine;

namespace AI
{
    public class BTRepeater : BTNode
    {
        private readonly BTNode _child;
        public BTRepeater(BTNode child)
        {
            _child = child;
        }

        public override Result Execute()
        {
            Debug.Log($"Child returned {_child.Execute()}");
            return Result.Running;
        }
    }
}