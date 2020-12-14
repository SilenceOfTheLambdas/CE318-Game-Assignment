using System;
using System.Collections.Generic;
using AI;
using AI.LeafNodes;
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
        /// <summary>
        ///     What layer is the ground on?
        /// </summary>
        [SerializeField] private LayerMask groundMask, playerMask;

        [SerializeField] private PlayerController player;
        
        #region Patrolling

        public Vector3 walkPoint;

        [SerializeField] private bool _walkPointSet;

        public float walkPointRange;

        #endregion

        #region Attacking

        public  float      timeBetweenAttacks;
        private bool       _alreadyAttacked;
        public  GameObject projectile;
        public  Transform  projectileSpawnPoint;

        #endregion

        #region States

        public float sightRange,         attackRange;
        public bool  playerInSightRange, playerInAttackRange;

        #endregion

        private Transform _bestCoverSpot;
        private Cover[]   _availableCovers;
        private BTNode    _rootNode;

        private NavMeshAgent _navMeshAgent;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            ConstructBehaviourTree();
        }

        private void ConstructBehaviourTree()
        {
            var behaviourTree = gameObject.AddComponent<BehaviourTree>();
            var coverAvailableNode = new IsCoverAvailableNode(behaviourTree, 
                _availableCovers, player.transform, this);
            var goToCoverNode = new GoToCoverNode(behaviourTree, this, _navMeshAgent);
            var healthNode = new HealthNode(behaviourTree, GetComponent<EnemyController>(), 
                GetComponent<EnemyController>().lowHealthThreshold);
            var isCoveredNode = new IsCoveredNode(behaviourTree, player.transform, transform);
            var chaseNode = new ChaseNode(behaviourTree, player.transform, _navMeshAgent);
            var chasingRangeNode = new RangeNode(behaviourTree, GetComponent<EnemyController>().chaseRange, 
                player.transform, transform);
            var shootingRangeNode = new RangeNode(behaviourTree, GetComponent<EnemyController>().shootRange, 
                player.transform, transform);
            var shootNode = new ShootNode(behaviourTree, _navMeshAgent, this);
            
            // Setting up the sequencer
            var chaseSequence = new BTSequencer(behaviourTree, new List<BTNode> { chasingRangeNode, chaseNode});
            var shootSequence = new BTSequencer(behaviourTree, new List<BTNode> { shootingRangeNode, shootNode});
            
            var goToCoverSequence = new BTSequencer(behaviourTree, new List<BTNode> { coverAvailableNode, goToCoverNode});
            var findCoverSelector = new BTSelector(behaviourTree, new List<BTNode> { goToCoverSequence, chaseSequence });
            var tryToTakeCoverSelector = new BTSelector(behaviourTree, new List<BTNode> { isCoveredNode, findCoverSelector });
            var mainCoverSequence = new BTSequencer(behaviourTree, new List<BTNode> {  healthNode, tryToTakeCoverSelector });
            
            _rootNode = new BTSelector(behaviourTree, new List<BTNode> { mainCoverSequence, shootSequence, chaseSequence });
        }

        private void Update()
        {
            /*// Check player sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

            // If player is not within the enemies' attack or sight range
            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            // If the player is withing sight range, but not attack range
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            // If the player is within the enemies' attack and sight range
            if (playerInAttackRange && playerInSightRange) AttackPlayer();*/

            if (_rootNode.Execute() == BTNode.Result.Failure)
            {
                GetComponent<EnemyController>().Die();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        /*private void Patrolling()
        {

            if (_walkPointSet) _navMeshAgent.SetDestination(walkPoint);

            var distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 2f) _walkPointSet = false;
        }*/

        /// <summary>
        ///     Search for a random area within the area specified by
        ///     <value>walkPointRange</value>
        /// </summary>
        /*private void SearchWalkPoint()
        {
            var randomZ = Random.Range(-walkPointRange, walkPointRange);
            var randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y,
                transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask)) _walkPointSet = true;
        }*/

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