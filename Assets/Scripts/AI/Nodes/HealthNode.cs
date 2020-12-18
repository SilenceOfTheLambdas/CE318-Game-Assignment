using Enemies;

namespace AI.Nodes
{
    public class HealthNode : BTNode
    {
        private readonly EnemyController _ai;
        private readonly float           _threshold;
        
        public HealthNode(EnemyController ai, float threshold)
        {
            _ai = ai;
            _threshold = threshold;
        }

        public override Result Execute()
        {
            return _ai.CurrentHealth <= _threshold ? Result.Success : Result.Failure;
        }
    }
}