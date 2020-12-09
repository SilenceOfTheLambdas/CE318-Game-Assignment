using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public KeyCode JumpKey, ShootKey, CrouchKey;

    [SerializeField] private float xAxis;

    [FormerlySerializedAs("yAxis")] [SerializeField]
    private float zAxis;

    [Header("References")] public PlayerController PlayerController;

    protected InputManager()
    {
    }

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                DontDestroyOnLoad(instance);
                instance = new InputManager();
            }

            return instance;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(JumpKey) && PlayerController.MovementState != PlayerController.MovementStates.Crouching)
            PlayerController.Jump();   

        if (Input.GetKeyDown(CrouchKey)) PlayerController.Crouch();

        if (Input.GetKeyDown(ShootKey)) PlayerController.Shoot();

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