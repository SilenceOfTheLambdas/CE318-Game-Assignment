using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// Similar to an AND gate, it will only run if all child nodes returned SUCCESS
    /// </summary>
    public class BTSelector : BTNode
    {
        private          int          _currentNode = 0;
        private readonly List<BTNode> _children;
        
        public BTSelector(IEnumerable<BTNode> children)
        {
            _children = new List<BTNode>(children);
        }

        public override Result Execute()
        {
            foreach (var node in _children)
            {
                switch (node.Execute())
                {
                    case Result.Running:
                        return Result.Running;
                    case Result.Success:
                        _currentNode++;
                        if (_currentNode < _children.Count)
                            return Result.Running;
                        _currentNode = 0;
                        return Result.Success;
                    case Result.Failure:
                        break;
                }   
            }
            return Result.Failure;
        }
    }
}