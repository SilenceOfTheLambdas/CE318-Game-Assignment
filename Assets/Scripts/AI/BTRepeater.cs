using UnityEngine;

namespace AI
{
    public class BTRepeater : BTDecorator
    {
        public BTRepeater(BehaviourTree t, BTNode child) : base(t, child)
        {
        }

        public override Result Execute()
        {
            Debug.Log($"Child returned {Child.Execute()}");
            return Result.Running;
        }
    }
}