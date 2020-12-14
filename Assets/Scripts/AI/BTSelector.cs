using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// Similar to an AND gate, it will only run if all child nodes returned SUCCESS
    /// </summary>
    public class BTSelector : BTComposite
    {
        private int _currentNode = 0;
        
        public BTSelector(BehaviourTree t, IEnumerable<BTNode> children) : base(t, children)
        {
        }

        public override Result Execute()
        {
            if (_currentNode < Children.Count)
            {
                var result = Children[_currentNode].Execute();

                switch (result)
                {
                    case Result.Running:
                        return Result.Running;
                    case Result.Success:
                        _currentNode++;
                        if (_currentNode < Children.Count)
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