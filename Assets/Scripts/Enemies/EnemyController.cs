using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Original implementation adapted from https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")] 
    public float hp;
    
    public enum MovementStates
    {
        Idle,
        Walking,
        Crouching,
        Running
    }
    public static MovementStates MovementState = MovementStates.Idle;
    
    [Range(1, 20)] [Header("Movement")]
    public float moveSpeed = 8f;
    public float defaultMoveSpeed;
    [Range(1, 20)]
    public float runSpeed = 12f;

    /// <summary>
    /// How long should the enemy wonder around a spot for? (in seconds)
    /// </summary>
    public float wonderTime;
    private float _timer;

    public WonderSpot[] wonderSpots;
    private NavMeshAgent _navMeshAgent;
    
    private void OnEnable()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _timer = wonderTime;
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;
        if (!HasReachedDestination()) return;
        if (_timer >= wonderTime)
        {
            var newSpot = wonderSpots[Random.Range(0, wonderSpots.Length)];
            _navMeshAgent.SetDestination(RandomPosition(newSpot));
            _timer = 0;
        }
    }

    /// <summary>
    /// Has the NavMesh agent reached it's desired destination?
    /// </summary>
    /// <returns>Reached destination</returns>
    private bool HasReachedDestination() => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;

    private Vector3 RandomPosition(WonderSpot spot)
    {
        var randomDirection = Random.insideUnitSphere * spot.WonderRadius;

        randomDirection += spot.Center.transform.position;

        NavMesh.SamplePosition(randomDirection, out var navHit, spot.WonderRadius, -1);

        return navHit.position;
    }

    private void OnCollisionEnter(Collision other)
    {
        // if a bullet collides with this enemy
        if (other.gameObject.CompareTag("bullet"))
        {
            var ammunitionManager = other.gameObject.GetComponent<AmmunitionManager>();
            hp -= ammunitionManager.damage;
            Debug.Log($"Enemy HP {hp}");
        }
    }
}
