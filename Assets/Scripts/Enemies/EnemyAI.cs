using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AI;
using AI.Nodes;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies
{
    /// <summary>
    ///     Script based on the implementation by Dave/GameDevelopment
    ///     (https://www.youtube.com/watch?v=UjkSFoLxesw)
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Animator         animator;
        
        // Movement
        protected float Forward;
        protected float Sideway;
        
        #region Patrolling

        [SerializeField] private bool    walkPointSet;
        public                   Vector3 walkPoint;
        public                   float   walkPointRange;
        private                  int     _groundMask;
        private                  int     _playerMask;

        #endregion

        #region Attacking

        public  float      timeBetweenAttacks;
        private bool       _alreadyAttacked;
        public  GameObject projectile;
        public  Transform  projectileSpawnPoint;

        #endregion

        #region States

        public float sightRange, attackRange;

        #endregion

        public  Cover[]   availableCovers;
        private Transform _bestCoverSpot;
        private BTNode    _rootNode;

        private                 NavMeshAgent _navMeshAgent;
        private static readonly int          Forward1 = Animator.StringToHash("Forward");
        private static readonly int          Sideway1 = Animator.StringToHash("Sideway");

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _groundMask = LayerMask.GetMask("Ground");
            _playerMask = LayerMask.GetMask("playerMask");
        }

        private void Start()
        {
            ConstructBehaviourTree();
        }

        [SuppressMessage("ReSharper", "Unity.IncorrectScriptableObjectInstantiation")]
        private void ConstructBehaviourTree()
        {
            // Nodes
            var coverAvailableNode = new IsCoverAvailableNode(availableCovers, player.transform, this);
            var goToCoverNode      = new GoToCoverNode(this, _navMeshAgent);
            var healthNode         = new HealthNode(GetComponent<EnemyController>(), 
                GetComponent<EnemyController>().lowHealthThreshold);
            var isCoveredNode     = new IsCoveredNode(player.transform, transform);
            var chaseNode         = new ChaseNode(player.transform, _navMeshAgent);
            var chasingRangeNode  = new RangeNode(GetComponent<EnemyController>().chaseRange, player.transform, transform);
            var shootingRangeNode = new RangeNode(GetComponent<EnemyController>().shootRange, player.transform, transform);
            var shootNode         = new ShootNode(_navMeshAgent, this);

            // Sequences
            var chaseSequence          = new BTSequencer(new List<BTNode> { chasingRangeNode, chaseNode });
            var shootSequence          = new BTSequencer(new List<BTNode> { shootingRangeNode, shootNode });
            var goToCoverSequence      = new BTSequencer(new List<BTNode> { coverAvailableNode, goToCoverNode });
            var findCoverSelector      = new BTSelector(new List<BTNode> { goToCoverSequence, chaseSequence });
            var tryToTakeCoverSelector = new BTSelector(new List<BTNode> { isCoveredNode, findCoverSelector });
            var mainCoverSequence      = new BTSequencer(new List<BTNode> { healthNode, tryToTakeCoverSelector });

            _rootNode = new BTSelector(new List<BTNode> { mainCoverSequence, shootSequence, chaseSequence });
        }

        private void Update()
        {
            // Check player sight and attack range
            var playerInSightRange = Physics.CheckSphere(transform.position, sightRange, _playerMask);
            var playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, _playerMask);

            // If player is not within the enemies' attack or sight range
            if (!playerInSightRange && !playerInAttackRange) Patrolling();

            var result = _rootNode.Execute();
            /*if (result == BTNode.Result.Failure)
            {
                GetComponent<EnemyController>().Die();
            }*/
        }

        private void FixedUpdate()
        {
            var movementVector = Vector3.Normalize(_navMeshAgent.desiredVelocity);
            // Update the movement variables
            Forward = -movementVector.x;
            Sideway = -movementVector.z;
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            animator.SetFloat(Forward1, Forward);
            animator.SetFloat(Sideway1, Sideway);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        private void Patrolling()
        {
            SearchWalkPoint();

            if (walkPointSet) _navMeshAgent.SetDestination(walkPoint);

            var distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 2f) walkPointSet = false;
        }

        /// <summary>
        ///     Search for a random area within the area specified by
        ///     <value>walkPointRange</value>
        /// </summary>
        private void SearchWalkPoint()
        {
            var randomZ = Random.Range(-walkPointRange, walkPointRange);
            var randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y,
                transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, _groundMask)) walkPointSet = true;
        }

        private void ChasePlayer()
        {
            _navMeshAgent.SetDestination(player.transform.position);
        }

        public void AttackPlayer()
        {
            // Stop the enemy from moving
            _navMeshAgent.SetDestination(transform.position);

            transform.LookAt(player.transform);

            if (_alreadyAttacked) return;
            if (player.isDead) return;
            // Attack
            var rb = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity)
                .GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10000000f, ForceMode.Acceleration);

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }

        public void SetBestCoverSpot(Transform bestSpot)
        {
            _bestCoverSpot = bestSpot;
        }

        public Transform GetBestCoverSpot()
        {
            return _bestCoverSpot;
        }
    }
}