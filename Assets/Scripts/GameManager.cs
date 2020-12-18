using Inventory;
using Player;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerEquipmentController playerEquipmentController;
    public                   InventoryUI               InventoryUI;
    public                   PauseMenu                 pauseMenuUI;
    public                   PlayerController          playerController;
    public                   Camera                    adsCamera;
    private                  bool                      _pauseMenuOpen;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (playerController.isDead)
        {
            deathMessage.text = "You Became Not Living.";
            playerController.enabled = false;
            deathBg.SetActive(true);
        }

        if (Input.GetButtonDown("Inventory"))
        {
            // Open inventory when user presses 'tab'
            if (!inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inventoryOpen = true;
                // Start Inventory System
                inventoryUI.gameObject.SetActive(true);
                playerEquipmentController.StartInventory();
            }

            if (inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                inventoryOpen = false;
                inventoryUI.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(InputManager.Instance.PauseMenuKey))
        {
            if (!_pauseMenuOpen)
            {
                // Open pause menu
                pauseMenuUI.OpenPauseMenu();
                _pauseMenuOpen = true;
            }

            if (_pauseMenuOpen)
            {
                pauseMenuUI.ClosePauseMenu();
                _pauseMenuOpen = false;
            }
        }
    }

    #region User Interface

    [Header("Inventory")] [SerializeField] private bool inventoryOpen;

    public InventoryUI inventoryUI;

    public GameObject      deathBg;
    public TextMeshProUGUI deathMessage;

    #endregion
}