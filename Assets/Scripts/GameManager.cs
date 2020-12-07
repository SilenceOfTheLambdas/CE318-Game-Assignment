using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerEquipmentController PlayerEquipmentController;
    public                   PlayerController          PlayerController;

    #region User Interface

    [Header("Inventory")] [SerializeField] private bool            inventoryOpen;
    [FormerlySerializedAs("DeathMessage")] public  TextMeshProUGUI deathMessage;
    [FormerlySerializedAs("DeathBG")]      public  GameObject      deathBg;
    [FormerlySerializedAs("InventoryUI")]  public  InventoryUI     inventoryUI;

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
                inventoryUI.GetComponent<Canvas>().gameObject.SetActive(true);
                PlayerEquipmentController.StartInventory();
                inventoryOpen = true;
            }
            else if (inventoryOpen)
            {
                inventoryUI.GetComponent<Canvas>().gameObject.SetActive(false);
                inventoryOpen = false;
            }
        }
    }
}