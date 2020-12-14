using Enemies;

namespace AI.LeafNodes
{
    public class HealthNode : BTNode
    {
        private readonly EnemyController ai;
        private readonly float           threshold;
        
        public HealthNode(BehaviourTree t, EnemyController ai, float threshold) : base(t)
        {
            this.ai = ai;
            this.threshold = threshold;
        }

        public override Result Execute()
        {
            return ai.CurrentHealth <= threshold ? Result.Success : Result.Failure;
        }
    }
}