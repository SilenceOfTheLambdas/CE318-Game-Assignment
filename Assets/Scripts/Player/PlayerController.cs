using System;
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

    [Header("References")] 
    public GameObject equippedWeapon;
    public Transform bulletSpawnPoint;
    public AmmunitionManager AmmunitionManager;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Transform topCheck;
    public LayerMask topMask;
    [FormerlySerializedAs("camera")] public Camera _camera;

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

    // Update is called once per frame

    private void Start()
    {
        _cameraOrigin = _camera.transform.localPosition;
        _defaultCharacterCenter = controller.center;
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
                GetComponent<Animator>().SetBool("isWalking", true);
                equippedWeapon.GetComponent<Animator>().SetBool("isRunning", false);
                moveSpeed = defaultMoveSpeed;
            }
            if ((xAxis != 0 || zAxis != 0) && MovementState != MovementStates.Crouching && MovementState != MovementStates.Running)
            {
                MovementState = MovementStates.Walking;
                GetComponent<Animator>().SetBool("isWalking", true);
                equippedWeapon.GetComponent<Animator>().SetBool("isRunning", false);
                moveSpeed = defaultMoveSpeed;
            }
            if ((xAxis != 0 || zAxis != 0) && Input.GetButtonDown("Sprint") && MovementState != MovementStates.Crouching)
            {
                MovementState = MovementStates.Running;
                moveSpeed = runSpeed;
                equippedWeapon.GetComponent<Animator>().SetBool("isRunning", true);
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
        if (Time.time > _nextFire && equippedWeapon.GetComponent<WeaponManager>().fireRate > 0)
        {
            _nextFire = Time.time + equippedWeapon.GetComponent<WeaponManager>().fireRate;
            
            equippedWeapon.GetComponent<Animator>().SetBool(IsPressingFire, true);
            
            var shot = Instantiate(AmmunitionManager.gameObject, bulletSpawnPoint.position,
                AmmunitionManager.gameObject.transform.localRotation);
            shot.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.transform.forward * AmmunitionManager.speed);
            equippedWeapon.GetComponent<AudioSource>().Play();
        }
        
        equippedWeapon.GetComponent<Animator>().SetBool("isRunning", MovementState == MovementStates.Running && IsGrounded);
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

    public void disableGunAnimation()
    {
        equippedWeapon.GetComponent<Animator>().SetBool(IsPressingFire, false);
    }
}
