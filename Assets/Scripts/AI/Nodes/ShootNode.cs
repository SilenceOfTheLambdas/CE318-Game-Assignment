using Enemies;
using UnityEngine.AI;

namespace AI.Nodes
{
    public class ShootNode : BTNode
    {
        private readonly NavMeshAgent _agent;
        private readonly EnemyAI      _enemyAI;
        private          bool         _alreadyAttacked;

        public ShootNode(NavMeshAgent agent, EnemyAI enemyAI)
        {
            _agent = agent;
            _enemyAI = enemyAI;
        }

        public override Result Execute()
        {
            _agent.isStopped = true;
            _enemyAI.AttackPlayer();
            return Result.Running;
        }
    }
}