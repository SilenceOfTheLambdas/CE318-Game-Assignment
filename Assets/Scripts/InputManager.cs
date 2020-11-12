using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    
    public KeyCode JumpKey, ShootKey, CrouchKey;

    [SerializeField] 
    private float xAxis;

    [FormerlySerializedAs("yAxis")] [SerializeField] 
    private float zAxis;

    [Header("References")] 
    public PlayerController PlayerController;

    protected InputManager() {}
    
    public static InputManager Instance
    {
        get
        {
            if (InputManager.instance == null)
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
        // Update axis
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(JumpKey) && PlayerController.MovementState != PlayerController.MovementStates.Crouching)
        {
            PlayerController.Jump();
        }
        
        PlayerController.Move(xAxis, zAxis);

        if (Input.GetKeyDown(CrouchKey))
        {
            PlayerController.Crouch();
        }

        if (Input.GetKeyDown(ShootKey))
        {
            PlayerController.Shoot();
        }

        if (Input.GetKeyUp(ShootKey))
        {
            PlayerController.disableGunAnimation();
        }
    }
}
