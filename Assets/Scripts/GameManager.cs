using Inventory;
using Player;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerEquipmentController PlayerEquipmentController;
    public                   PlayerController          PlayerController;
    public                   Camera                    ADSCamera;

    #region User Interface

    [Header("Inventory")] 
    [SerializeField] 
    private bool        inventoryOpen;
    public  InventoryUI inventoryUI;
    
    public  GameObject      deathBg;
    public  TextMeshProUGUI deathMessage;

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
    }

    private void Update()
    {
        if (PlayerController.isDead)
        {
            deathMessage.text = "You Became Not Living.";
            PlayerController.enabled = false;
            deathBg.SetActive(true);
        }

        if (Input.GetButtonDown("Inventory"))
        {
            // Open inventory when user presses 'tab'
            if (!inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                inventoryOpen = true;
                // Start Inventory System
                inventoryUI.gameObject.SetActive(true);
                PlayerEquipmentController.StartInventory();
            }
            if (inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                inventoryOpen = false;
                inventoryUI.gameObject.SetActive(false);
            }
        }
    }
}