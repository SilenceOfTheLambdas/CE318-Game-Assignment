using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
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

        [Header("Enemy Stats")] 
        private float _currentHealth;

        public float startingHealth;
        public float lowHealthThreshold;
        public float healthRestoreRate;
        public float chaseRange;
        public float shootRange;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set => _currentHealth = Mathf.Clamp(value, 0, startingHealth);
        }


        [Range(1, 20)] [Header("Movement")] 
        public float moveSpeed = 8f;
        [Range(1, 20)] public float runSpeed = 12f;
        public                float defaultMoveSpeed;


        private void Start()
        {
            CurrentHealth = startingHealth;
        }

        // Update is called once per frame
        private void Update()
        {
            if (startingHealth <= 0) Die();

            CurrentHealth += Time.deltaTime * healthRestoreRate;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("bullet"))
            {
                var ammunitionManager = other.gameObject.GetComponent<AmmunitionManager>();
                startingHealth -= ammunitionManager.damage;
                Destroy(other.gameObject);
            }
        }

        public void Die()
        {
            GetComponent<Renderer>().material.color = Color.black;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<EnemyAI>().enabled = false;
            GetComponent<NavMeshAgent>().isStopped = true;
        }
    }
}