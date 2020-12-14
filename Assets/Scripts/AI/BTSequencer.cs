using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// Similar to an AND gate, it will only run if all child nodes returned SUCCESS
    /// </summary>
    public class BTSequencer : BTComposite
    {
        private int _currentNode = 0;
        
        public BTSequencer(BehaviourTree t, IEnumerable<BTNode> children) : base(t, children)
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
                    case Result.Failure:
                        _currentNode = 0;
                        return Result.Failure;
                    default:
                    {
                        _currentNode++;
                        if (_currentNode < Children.Count)
                            return Result.Running;
                        _currentNode = 0;
                        return Result.Success;
                    }
                }
            }

            return Result.Success;
        }
    }
}