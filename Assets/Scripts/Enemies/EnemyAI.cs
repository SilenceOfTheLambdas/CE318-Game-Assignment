using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script based on the implementation by Dave/GameDevelopment
/// (https://www.youtube.com/watch?v=UjkSFoLxesw)
/// </summary>
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform player;

    [SerializeField] 
    private LayerMask groundMask, playerMask;

    #region Patrolling
    public Vector3 walkPoint;
    private bool _walkPointSet;
    public float walkPointRange;
    #endregion

    #region Attacking
    public float timeBetweenAttacks;
    private bool _alreadyAttacked;
    public GameObject projectile;
    public Transform projectileSpawnPoint;
    #endregion

    #region States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    #endregion

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check player sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        
        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
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

        if (distanceToWalkPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        var randomZ = Random.Range(-walkPointRange, walkPointRange);
        var randomX = Random.Range(-walkPointRange, walkPointRange);
        
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        _walkPointSet = Physics.Raycast(walkPoint, -transform.up, groundMask);
    }

    private void ChasePlayer()
    {
        _navMeshAgent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Stop the enemy from moving
        _navMeshAgent.SetDestination(transform.position);
        
        transform.LookAt(player);

        if (_alreadyAttacked) return;
        // Attack
        var rb = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 10000000f, ForceMode.Acceleration);
        //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        //

        _alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack() => _alreadyAttacked = false;
}
