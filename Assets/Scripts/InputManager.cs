using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_Systems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public KeyCode JumpKey, ShootKey, CrouchKey;

    [SerializeField] private float xAxis;

    [FormerlySerializedAs("yAxis")] [SerializeField]
    private float zAxis;

    [Header("References")] public PlayerController PlayerController;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(JumpKey) && PlayerController.MovementState != PlayerController.MovementStates.Crouching)
            PlayerController.Jump();   

        if (Input.GetKeyDown(CrouchKey)) PlayerController.Crouch();

        if (Input.GetMouseButton(0)) PlayerController.Shoot();

        if (Input.GetKeyUp(ShootKey))
        {
            PlayerController.primaryWeapon.GetComponent<WeaponManager>().DisableGunAnimation();
        }
        
        // Player Movement
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        // Update axis
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        
        PlayerController.Move(xAxis, zAxis);
    }
}