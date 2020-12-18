using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// Similar to an AND gate, it will only run if all child nodes returned SUCCESS
    /// </summary>
    public class BTSequencer : BTNode
    {
        private readonly List<BTNode> _children;
        
        public BTSequencer(IEnumerable<BTNode> children)
        {
            _children = new List<BTNode>(children);
        }

        public override Result Execute()
        {
            var isAnyNodeRunning = false;
            foreach (var node in _children)
            {
                switch (node.Execute())
                {
                    case Result.Running:
                        isAnyNodeRunning = true;
                        break;
                    case Result.Success:
                        break;
                    case Result.Failure:
                        return Result.Failure;
                }
            }

            return isAnyNodeRunning ? Result.Running : Result.Success;
        }
    }
}