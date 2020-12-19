using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Nodes
{
    public class WonderNode : BTNode
    {
        private readonly NavMeshAgent    _agent;
        private readonly GameObject      _enemy;
        private readonly EnemyController _enemyController;
        private          Vector3         _walkPoint;
        private          bool            _walkPointSet;
        private readonly int             _groundMask;

        public WonderNode(NavMeshAgent agent, GameObject enemy)
        {
            _agent = agent;
            _enemy = enemy;
            _enemyController = _enemy.GetComponent<EnemyController>();
            _groundMask = LayerMask.GetMask("Ground");
        }

        public override Result Execute()
        {
            if (!_walkPointSet) SearchWalkPoint();

            if (_walkPointSet)
            {
                _agent.SetDestination(_walkPoint);
            }

            var distanceToWalkPoint = _enemy.transform.position - _walkPoint;

            if (distanceToWalkPoint.magnitude < 2f)
            {
                _walkPointSet = false;
                return Result.Success;
            }

            return Result.Running;
        }

        /// <summary>
        ///     Search for a random area within the area specified by
        ///     <value>walkPointRange</value>
        /// </summary>
        private void SearchWalkPoint()
        {
            var randomZ = Random.Range(-_enemyController.walkPointRange, _enemyController.walkPointRange);
            var randomX = Random.Range(-_enemyController.walkPointRange, _enemyController.walkPointRange);

            _walkPoint = new Vector3(_enemy.transform.position.x + randomX, _enemy.transform.position.y,
                _enemy.transform.position.z + randomZ);

            if (Physics.Raycast(_walkPoint, -_enemy.transform.up, 2f, _groundMask))
            {
                _walkPointSet = true;
            }
        }
    }
}