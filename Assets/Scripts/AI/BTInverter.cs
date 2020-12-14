using System;

namespace AI
{
    public class BTInverter : BTNode
    {
        protected BTNode _node;
        
        public BTInverter(BehaviourTree t) : base(t)
        {
        }

        public override Result Execute()
        {
            switch (_node.Execute())
            {
                case Result.Running:
                    return Result.Running;
                case Result.Failure:
                    return Result.Success;
                case Result.Success:
                    return Result.Failure;
            }

            return Result.Failure;
        }
    }
}