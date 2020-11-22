using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public enum MovementStates
    {
        Idle,
        Walking,
        Crouching,
        Running
    }
    public static MovementStates MovementState = MovementStates.Idle;
    
    // List of player "States"; this controls the position of the arms etc. when a weapon is equipped
    public Dictionary<string, GameObject> playerEquippedStates;

    [Range(1, 20)] [Header("Movement")]
    public float moveSpeed = 8f;
    public float defaultMoveSpeed;
    [Range(1, 20)]
    public float runSpeed = 12f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float jumpHeight = 3f;
    public float crouchHeight = 1f;
    public float crouchMovementSpeed = 6f;

    [FormerlySerializedAs("equippedWeapon")] [Header("Equipped Weapons")]
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    public GameObject thirdWeapon;
    
    [Header("References")] 
    public Transform bulletSpawnPoint;
    public AmmunitionManager AmmunitionManager;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Transform topCheck;
    public LayerMask topMask;
    [FormerlySerializedAs("camera")] public Camera _camera;
    [SerializeField] private Inventory inventory;

    [SerializeField] 
    private float health;
    
    public static bool IsGrounded;
    public float movementCounter;
    public bool isDead;
    
    // Private Variables
    private Vector3 _velocity;
    private Vector3 _defaultCharacterCenter;
    private Vector3 _cameraOrigin;
    private static readonly int IsPressingFire = Animator.StringToHash("isPressingFire");
    private float _nextFire;
    private Animator _playerAnimator;
    private Animator _equippedWeaponAnimator;

    // Update is called once per frame

    private void Start()
    {
        primaryWeapon = GameObject.Find("WPN_AKM(Clone)");
        _playerAnimator = GetComponent<Animator>();
        _cameraOrigin = _camera.transform.localPosition;
        _defaultCharacterCenter = controller.center;
//        _equippedWeaponAnimator = primaryWeapon.GetComponent<Animator>();
        defaultMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (IsGrounded)
        {
            if (_velocity.y < 0)
                _velocity.y = -2f;
        }
        
        _velocity.y += gravity * Time.deltaTime;
        
        // Velocity calculation requires time(2)
        controller.Move(_velocity * Time.deltaTime);
        
        // If player stops crouching
        if (Input.GetButtonUp("Crouch")) {

            if (!Physics.CheckSphere(topCheck.position, 0.5f, topMask))
            {
                controller.height = 2;
                controller.center = _defaultCharacterCenter;
                moveSpeed = 12f;
                _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, 
                    _cameraOrigin.y, _camera.transform.localPosition.z);
                MovementState = MovementStates.Idle;
            }
        }
        
        // Open inventory when user presses 'tab'
        if (Input.GetButtonDown("Inventory"))
        {
            GetComponent<PlayerEquipmentController>().StartInventory();
        }
    }

    public void Jump() => 
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

    public void Move(float xAxis, float zAxis)
    {
        if (IsGrounded)
        {   
            if (Input.GetButtonUp("Sprint"))
            {
                MovementState = MovementStates.Walking;
                _playerAnimator.SetBool("isWalking", true);
                _playerAnimator.SetBool("isRunning", false);
//                _equippedWeaponAnimator.SetBool("isRunning", false);
                moveSpeed = defaultMoveSpeed;
            }
            if ((xAxis != 0 || zAxis != 0) && MovementState != MovementStates.Crouching && MovementState != MovementStates.Running)
            {
                MovementState = MovementStates.Walking;
                _playerAnimator.SetBool("isWalking", true);
//                _equippedWeaponAnimator.SetBool("isRunning", false);
                moveSpeed = defaultMoveSpeed;
            }
            if ((xAxis != 0 || zAxis != 0) && Input.GetButtonDown("Sprint") && MovementState != MovementStates.Crouching)
            {
                Debug.Log("Running...");
                MovementState = MovementStates.Running;
                moveSpeed = runSpeed;
                _equippedWeaponAnimator.SetBool("isRunning", true);
                _playerAnimator.SetBool("isRunning", true);
            }
        }

        var transform1 = transform;
        var move = transform1.right * xAxis + transform1.forward * zAxis;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void Crouch()
    {
        controller.height = crouchHeight;
        moveSpeed = crouchMovementSpeed;
        MovementState = MovementStates.Crouching;
            
        _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, -0.3f, _camera.transform.localPosition.z);
    }

    public void Shoot()
    {
        if (Time.time > _nextFire && primaryWeapon.GetComponent<WeaponManager>().fireRate > 0)
        {
            _nextFire = Time.time + primaryWeapon.GetComponent<WeaponManager>().fireRate;
            
            _equippedWeaponAnimator.SetBool(IsPressingFire, true);
            
            var shot = Instantiate(AmmunitionManager.gameObject, bulletSpawnPoint.position,
                AmmunitionManager.gameObject.transform.localRotation);
            shot.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.transform.forward * AmmunitionManager.speed);
            primaryWeapon.GetComponent<AudioSource>().Play();
        }
        
        _equippedWeaponAnimator.SetBool("isRunning", MovementState == MovementStates.Running && IsGrounded);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            if (health <= 0) isDead = true;
            
            var bullet = other.gameObject;
            health -= bullet.GetComponent<AmmunitionManager>().damage;
        }
    }

    public void DisableGunAnimation()
    {
//        _equippedWeaponAnimator.SetBool(IsPressingFire, false);
    }
}
