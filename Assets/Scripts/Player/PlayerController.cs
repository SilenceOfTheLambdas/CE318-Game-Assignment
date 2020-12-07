using System.Collections.Generic;
using UnityEngine;

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

    public static           bool IsGrounded;
    private static readonly int  IsRunning = Animator.StringToHash("isRunning");
    private static readonly int  IsWalking = Animator.StringToHash("isWalking");

    [Range(1, 20)] [Header("Movement")] public float moveSpeed = 8f;

    public float defaultMoveSpeed;

    [Range(1, 20)] public float runSpeed = 12f;

    public float gravity             = -9.81f;
    public float groundDistance      = 0.4f;
    public float jumpHeight          = 3f;
    public float crouchHeight        = 1f;
    public float crouchMovementSpeed = 6f;

    [Header("Equipped Weapons")] public GameObject primaryWeapon;

    public GameObject secondaryWeapon;
    public GameObject thirdWeapon;

    [Header("References")] public CharacterController controller;

    public Transform groundCheck;
    public LayerMask groundMask;
    public Transform topCheck;
    public LayerMask topMask;
    public Camera    _camera;

    [SerializeField] private float health;

    public  float    movementCounter;
    public  bool     isDead;
    private Vector3  _cameraOrigin;
    private Vector3  _defaultCharacterCenter;
    private float    _nextFire;
    private Animator _playerAnimator;

    // Private Variables
    private Vector3 _velocity;

    // List of player "States"; this controls the position of the arms etc. when a weapon is equipped
    public Dictionary<string, GameObject> playerEquippedStates;

    // Update is called once per frame
    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _cameraOrigin = _camera.transform.localPosition;
        _defaultCharacterCenter = controller.center;
        defaultMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (IsGrounded)
            if (_velocity.y < 0)
                _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;

        // Velocity calculation requires time(2)
        controller.Move(_velocity * Time.deltaTime);

        // If player stops crouching
        if (Input.GetButtonUp("Crouch"))
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            if (health <= 0) isDead = true;

            var bullet = other.gameObject;
            health -= bullet.GetComponent<AmmunitionManager>().damage;
        }
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void Move(float xAxis, float zAxis)
    {
        if (IsGrounded)
        {
            if (Input.GetButtonUp("Sprint"))
            {
                MovementState = MovementStates.Walking;
                _playerAnimator.SetBool(IsWalking, true);
                _playerAnimator.SetBool(IsRunning, false);
                if (primaryWeapon != null)
                    primaryWeapon.GetComponent<Animator>().SetBool(IsRunning, false);
                moveSpeed = defaultMoveSpeed;
            }

            if ((xAxis != 0 || zAxis != 0) && MovementState != MovementStates.Crouching &&
                MovementState != MovementStates.Running)
            {
                MovementState = MovementStates.Walking;
                _playerAnimator.SetBool(IsWalking, true);
                if (primaryWeapon != null)
                    primaryWeapon.GetComponent<Animator>().SetBool(IsRunning, false);
                moveSpeed = defaultMoveSpeed;
            }

            if ((xAxis != 0 || zAxis != 0) && Input.GetButtonDown("Sprint") &&
                MovementState != MovementStates.Crouching)
            {
                Debug.Log("Running...");
                MovementState = MovementStates.Running;
                moveSpeed = runSpeed;
                if (primaryWeapon != null)
                    primaryWeapon.GetComponent<Animator>().SetBool(IsRunning, true);
                _playerAnimator.SetBool(IsRunning, true);
            }
        }

        var transform1 = transform;
        var move       = transform1.right * xAxis + transform1.forward * zAxis;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void Crouch()
    {
        controller.height = crouchHeight;
        moveSpeed = crouchMovementSpeed;
        MovementState = MovementStates.Crouching;

        _camera.transform.localPosition =
            new Vector3(_camera.transform.localPosition.x, -0.3f, _camera.transform.localPosition.z);
    }

    public void Shoot()
    {
        if (primaryWeapon != null)
            primaryWeapon.GetComponent<WeaponManager>().Shoot();
    }
}