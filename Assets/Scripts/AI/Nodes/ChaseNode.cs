using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Nodes
{
    public class ChaseNode : BTNode
    {
        private readonly Transform    _target;
        private readonly NavMeshAgent _agent;

        public ChaseNode(Transform target, NavMeshAgent agent)
        {
            _target = target;
            _agent = agent;
        }

        public override Result Execute()
        {
            var distance = Vector3.Distance(_target.position, _agent.transform.position);
            
            if (distance > 0.2f)
            {
                _agent.isStopped = false;
                _agent.speed = _agent.gameObject.GetComponent<EnemyController>().runSpeed;
                //_agent.gameObject.GetComponent<Animator>().speed *= _agent.gameObject.GetComponent<EnemyController>().runSpeed;
                _agent.SetDestination(_target.position);
                return Result.Running;
            }

            _agent.isStopped = true;
            _agent.speed = _agent.gameObject.GetComponent<EnemyController>().defaultMoveSpeed;
            //_agent.gameObject.GetComponent<Animator>().speed *= _agent.gameObject.GetComponent<EnemyController>().defaultMoveSpeed;
            return Result.Success;
        }
    }
}