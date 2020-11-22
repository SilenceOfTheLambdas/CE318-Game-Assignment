using UnityEngine;

/// <summary>
/// Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
/// Adjusted by 1806094
/// </summary>
public class PlayerEquipmentController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform inventoryUIParent;

    [Header("Anchors")] [SerializeField] private Transform weaponAnchor;

    private GameObject _currentEquippedWeapon;

    
    public void StartInventory()
    {
        inventory.InitInventory(this);
        inventory.OpenInventoryUI();
    }

    public void AssignRifleItem(RifleInventoryItem rifleInventoryItem)
    {
        DestroyIfNotNull(_currentEquippedWeapon);
        _currentEquippedWeapon = CreateNewItemInstance(rifleInventoryItem, weaponAnchor);
    }

    private GameObject CreateNewItemInstance(InventoryItem item, Transform anchor)
    {
        var itemInstance = Instantiate(item.GetPrefab(), anchor);
        itemInstance.transform.localPosition = item.GetLocalPosition();
        itemInstance.transform.localRotation = item.GetLocalRotation();
        return itemInstance;
    }

    private void DestroyIfNotNull(GameObject obj)
    {
        if (obj)
            Destroy(obj);
    }

    public Transform GetUIParent() => inventoryUIParent;

}
