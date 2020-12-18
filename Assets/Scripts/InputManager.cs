using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_Systems;

public class InputManager : MonoBehaviour
{
    public KeyCode JumpKey, ShootKey, CrouchKey, PickupKey, PauseMenuKey;

    [SerializeField] private float xAxis;

    [FormerlySerializedAs("yAxis")] [SerializeField]
    private float zAxis;

    [Header("References")] public PlayerController PlayerController;
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(JumpKey) && PlayerController.MovementState != PlayerController.MovementStates.Crouching
                                      && PlayerController.IsGrounded)
            PlayerController.Jump();

        if (Input.GetKeyDown(CrouchKey)) PlayerController.Crouch();


        if (Input.GetKeyUp(ShootKey))
            if (PlayerController.primaryWeapon != null)
                PlayerController.primaryWeapon.GetComponent<WeaponManager>().DisableGunAnimation();

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