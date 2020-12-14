﻿using System;
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

        [Header("Enemy Stats")] [SerializeField]
        public float startingHealth;
        [SerializeField] public  float lowHealthThreshold;
        [SerializeField] public  float healthRestoreRate;
        [SerializeField] public  float chaseRange;
        [SerializeField] public  float shootRange;

        public float CurrentHealth
        {
            get => CurrentHealth;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                CurrentHealth = Mathf.Clamp(value, 0, startingHealth);
            }
        }


        [Range(1, 20)] [Header("Movement")] public float moveSpeed = 8f;

        public float defaultMoveSpeed;

        [Range(1, 20)] public float runSpeed = 12f;


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

        private void OnTriggerEnter(Collider other)
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