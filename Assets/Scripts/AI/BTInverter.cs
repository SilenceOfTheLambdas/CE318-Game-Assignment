using System;

namespace AI
{
    public class BTInverter : BTNode
    {
        private readonly BTNode _node;
        
        public BTInverter(BTNode node)
        {
            _node = node;
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