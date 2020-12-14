using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Nodes
{
    public class GoToCoverNode : BTNode
    {
        private readonly NavMeshAgent _agent;
        private          EnemyAI      _enemyAI;

        public GoToCoverNode(BehaviourTree t, EnemyAI enemyAI, NavMeshAgent agent) : base(t)
        {
            _agent = agent;
            _enemyAI = enemyAI;
        }

        public override Result Execute()
        {
            Transform cover    = _enemyAI.GetBestCoverSpot();
            var       distance = Vector3.Distance(cover.position, _agent.transform.position);
            if (cover == null)
                return Result.Failure;
            
            if (distance > 0.2f)
            {
                _agent.isStopped = false;
                _agent.SetDestination(cover.position);
                return Result.Running;
            }
            else
            {
                _agent.isStopped = true;
                return Result.Success;
            }
        }
    }
}