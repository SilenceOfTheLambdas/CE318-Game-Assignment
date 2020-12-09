using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    /// <summary>
    ///     Script based on the implementation by Dave/GameDevelopment
    ///     (https://www.youtube.com/watch?v=UjkSFoLxesw)
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        /// <summary>
        /// What layer is the ground on?
        /// </summary>
        [SerializeField] private LayerMask groundMask, playerMask;

        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private PlayerController player;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            // Check player sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

            // If player is not within the enemies' attack or sight range
            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            // If the player is withing sight range, but not attack range
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            // If the player is within the enemies' attack and sight range
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        private void Patrolling()
        {
            if (!_walkPointSet) SearchWalkPoint();

            if (_walkPointSet)
            {
                _navMeshAgent.SetDestination(walkPoint);
            }

            var distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 2f)
            {
                _walkPointSet = false;
            }
        }

        /// <summary>
        /// Search for a random area within the area specified by <value>walkPointRange</value>
        /// </summary>
        private void SearchWalkPoint()
        {
            var randomZ = Random.Range(-walkPointRange, walkPointRange);
            var randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
            {
                _walkPointSet = true;
            }
        }

        private void ChasePlayer()
        {
            _navMeshAgent.SetDestination(player.transform.position);
        }

        private void AttackPlayer()
        {
            // Stop the enemy from moving
            _navMeshAgent.SetDestination(transform.position);

            transform.LookAt(player.transform);

            if (_alreadyAttacked) return;
            if (player.isDead) return;
            // Attack
            var rb = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10000000f, ForceMode.Acceleration);

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        #region Patrolling

        public Vector3 walkPoint;
        [SerializeField]
        private bool _walkPointSet;
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
    }
}