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
    public MovementStates movementState = MovementStates.Idle;
    
    [Header("Movement")]
    public float moveSpeed = 12f;
    private float _defaultMoveSpeed;
    public float runSpeed = 8f;
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
    
    private Vector3 _velocity;
    private bool _isGrounded;
    private Vector3 _defaultCharacterCenter;
    private float _defaultCharacterHeight;
    
    // Update is called once per frame

    private void Start()
    {
        _defaultCharacterHeight = controller.height;
        _defaultCharacterCenter = controller.center;
        _defaultMoveSpeed = moveSpeed;
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Move();
        if (_isGrounded)
        {
            Crouch();
            if (movementState != MovementStates.Crouching)
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

        if ((x != 0 || z != 0) && movementState != MovementStates.Crouching)
        {
            movementState = MovementStates.Walking;
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
            controller.center = new Vector3(0f, -0.5f, 0f);
            moveSpeed = crouchMovementSpeed;
            movementState = MovementStates.Crouching;
            
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight / 2, transform.localScale.z);
        }
        
        if (Input.GetButtonUp("Crouch")) {

            if (!Physics.CheckSphere(topCheck.position, 0.5f, topMask))
            {
                controller.height = 2;
                controller.center = _defaultCharacterCenter;
                moveSpeed = 12f;
                transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
                movementState = MovementStates.Idle;
            }
        }
    }
}
