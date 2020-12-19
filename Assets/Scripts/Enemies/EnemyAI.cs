using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AI;
using AI.Nodes;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Weapon_Systems;

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
        [SerializeField] private GameObject       equippedWeapon;
        
        // Movement
        private float _forward;
        private float _sideway;

        #region Attacking

        private bool       _alreadyAttacked;

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
            _navMeshAgent.speed = GetComponent<EnemyController>().moveSpeed;
        }

        private void Start()
        {
            ConstructBehaviourTree();
        }

        [SuppressMessage("ReSharper", "Unity.IncorrectScriptableObjectInstantiation")]
        private void ConstructBehaviourTree()
        {
            // Nodes
            var wonderNode         = new WonderNode(_navMeshAgent, gameObject);
            var coverAvailableNode = new IsCoverAvailableNode(availableCovers, player.transform, this);
            var goToCoverNode      = new GoToCoverNode(this, _navMeshAgent);
            var healthNode         = new HealthNode(GetComponent<EnemyController>(), 
                GetComponent<EnemyController>().lowHealthThreshold);
            var isCoveredNode = new IsCoveredNode(player.transform, transform);
            var chaseNode     = new ChaseNode(player.transform, _navMeshAgent);
            var wonderRangeNode =
                new NotInRangeNode(GetComponent<EnemyController>().chaseRange, player.transform, transform);
            var chasingRangeNode  = new RangeNode(GetComponent<EnemyController>().chaseRange, player.transform, transform);
            var shootingRangeNode = new RangeNode(GetComponent<EnemyController>().shootRange, player.transform, transform);
            var shootNode         = new ShootNode(_navMeshAgent, this);

            // Sequences
            var wonderSequence         = new BTSequencer(new List<BTNode> { wonderRangeNode, wonderNode});
            var chaseSequence          = new BTSequencer(new List<BTNode> { chasingRangeNode, chaseNode });
            var shootSequence          = new BTSequencer(new List<BTNode> { shootingRangeNode, shootNode });
            var goToCoverSequence      = new BTSequencer(new List<BTNode> { coverAvailableNode, goToCoverNode });
            var findCoverSelector      = new BTSelector(new List<BTNode> { goToCoverSequence, chaseSequence });
            var tryToTakeCoverSelector = new BTSelector(new List<BTNode> { isCoveredNode, findCoverSelector });
            var mainCoverSequence      = new BTSequencer(new List<BTNode> { healthNode, tryToTakeCoverSelector });

            _rootNode = new BTSelector(new List<BTNode> { 
                wonderSequence,
                mainCoverSequence,
                shootSequence,
                chaseSequence });
        }

        private void Update()
        {
            if (GetComponent<EnemyController>().CurrentHealth > 0)
                _rootNode.Execute();
        }

        private void FixedUpdate()
        {
            var movementVector = Vector3.Normalize(_navMeshAgent.desiredVelocity);
            // Update the movement variables
            _forward = -movementVector.x;
            _sideway = -movementVector.z;
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            animator.SetFloat(Forward1, _forward);
            animator.SetFloat(Sideway1, _sideway);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        public void AttackPlayer()
        {
            transform.LookAt(player.transform.position);

            if (player.isDead) return;

            equippedWeapon.GetComponent<WeaponManager>().Shoot();
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