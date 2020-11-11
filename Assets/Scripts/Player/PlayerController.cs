using System;
using System.Collections;
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
    [SerializeField]
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
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Transform topCheck;
    public LayerMask topMask;
    public Camera camera;
    [Header("Weapons")]
    public GameObject gun;
    public GameObject bulletPrefab;
    public Transform bulletHolder;

    private Vector3 _velocity;
    public static bool IsGrounded;
    private Vector3 _defaultCharacterCenter;
    private float _defaultCharacterHeight;
    
    public float movementCounter = 0;
    public float idleCounter = 0;

    private Vector3 _cameraOrigin;
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    // Update is called once per frame

    private void Start()
    {
        _cameraOrigin = camera.transform.localPosition;
        _defaultCharacterHeight = controller.height;
        _defaultCharacterCenter = controller.center;
        defaultMoveSpeed = moveSpeed;
    }

    void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Move();
        if (IsGrounded)
        {
            Crouch();
            if (MovementState != MovementStates.Crouching)
                Jump();
            if (_velocity.y < 0)
                _velocity.y = -2f;
        }
        
        _velocity.y += gravity * Time.deltaTime;
        
        // Velocity calculation requires time(2)
        controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Move()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        if (IsGrounded)
        {
            if (Input.GetButtonUp("Sprint"))
            {
                MovementState = MovementStates.Walking;
                GetComponent<Animator>().SetBool("isWalking", true);
                moveSpeed = defaultMoveSpeed;
            }
            if ((x != 0 || z != 0) && MovementState != MovementStates.Crouching && MovementState != MovementStates.Running)
            {
                MovementState = MovementStates.Walking;
                GetComponent<Animator>().SetBool("isWalking", true);
                moveSpeed = defaultMoveSpeed;
            }
            if ((x != 0 || z != 0) && Input.GetButtonDown("Sprint") && MovementState != MovementStates.Crouching)
            {
                MovementState = MovementStates.Running;
                moveSpeed = runSpeed;
            }
        }

        var transform1 = transform;
        var move = transform1.right * x + transform1.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            controller.height = crouchHeight;
            moveSpeed = crouchMovementSpeed;
            MovementState = MovementStates.Crouching;
            
            camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, -0.3f, camera.transform.localPosition.z);
        }
        
        if (Input.GetButtonUp("Crouch")) {

            if (!Physics.CheckSphere(topCheck.position, 0.5f, topMask))
            {
                controller.height = 2;
                controller.center = _defaultCharacterCenter;
                moveSpeed = 12f;
                camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, 
                    _cameraOrigin.y, camera.transform.localPosition.z);
                MovementState = MovementStates.Idle;
            }
        }
    }
}
