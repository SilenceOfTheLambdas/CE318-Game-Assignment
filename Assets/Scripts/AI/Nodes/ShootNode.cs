using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.LeafNodes
{
    public class ShootNode : BTNode
    {
        private readonly NavMeshAgent _agent;
        private readonly EnemyAI      _enemyAI;
        private          bool         _alreadyAttacked;

        public ShootNode(BehaviourTree t, NavMeshAgent agent, EnemyAI enemyAI) : base(t)
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