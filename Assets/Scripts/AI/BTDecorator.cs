namespace AI
{
    public class BTDecorator : BTNode
    {
        public BTNode Child { get; set; }
        public BTDecorator(BehaviourTree t, BTNode child) : base(t)
        {
            Child = child;
        }
        
        
    }
}