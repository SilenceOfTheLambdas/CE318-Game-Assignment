using Enemies;
using UnityEngine;

/// <summary>
///     Original implementation adapted from https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950
/// </summary>
public class EnemyController : MonoBehaviour
{
    public enum MovementStates
    {
        Idle,
        Walking,
        Crouching,
        Running
    }

    public static MovementStates MovementState = MovementStates.Idle;

    //private Animator _animator;
    private static readonly int IsDead = Animator.StringToHash("isDead");

    [Header("Enemy Stats")] public float hp;

    [Range(1, 20)] [Header("Movement")] public float moveSpeed = 8f;

    public float defaultMoveSpeed;

    [Range(1, 20)] public float runSpeed = 12f;


    // Update is called once per frame
    private void Update()
    {
        if (hp <= 0) Die();
    }

    private void OnCollisionEnter(Collision other)
    {
        // if a bullet collides with this enemy
        if (other.gameObject.CompareTag("bullet"))
        {
            var ammunitionManager = other.gameObject.GetComponent<AmmunitionManager>();
            hp -= ammunitionManager.damage;
        }
    }

    private void Die()
    {
        GetComponent<Renderer>().material.color = Color.black;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<EnemyAI>().enabled = false;
    }
}