using System.Collections.Generic;

namespace AI
{
    public class BTComposite : BTNode
    {
        public List<BTNode> Children { get; set; }
        public BTComposite(BehaviourTree t, IEnumerable<BTNode> nodes) : base(t)
        {
            Children = new List<BTNode>(nodes);
        }
    }
}