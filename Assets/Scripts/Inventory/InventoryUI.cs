using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
///     Adjusted by 1806094
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform     slotsParent;
    [SerializeField] private InventorySlot slotPrefab;

    private readonly Dictionary<InventoryItem, InventorySlot> itemToSlotMap =
        new Dictionary<InventoryItem, InventorySlot>();

    private static readonly int InventoryOpen = Animator.StringToHash("inventoryOpen");

    public void InitInventory(Inventory.Inventory inventory)
    {
        GetComponent<Animator>().SetBool(InventoryOpen, true);
        var itemsMap = inventory.GetAllItemsMap();
        foreach (var kvp in itemsMap) CreateOrUpdateSlot(inventory, kvp.Key, kvp.Value);
    }

    public void CreateOrUpdateSlot(Inventory.Inventory inventory, InventoryItem inventoryItem, int count)
    {
        if (!itemToSlotMap.ContainsKey(inventoryItem))
        {
            var slot = CreateSlot(inventory, inventoryItem, count);
            itemToSlotMap.Add(inventoryItem, slot);
        }
        else
        {
            UpdateSlot(inventoryItem, count);
        }
    }

    private InventorySlot CreateSlot(Inventory.Inventory inventory, InventoryItem inventoryItem, int count)
    {
        var slot = Instantiate(slotPrefab, slotsParent);
        slot.InitSlotVisualisation(inventoryItem.GetSprite(), inventoryItem.GetName(), count);
        slot.AssignSlotButtonCallback(() => inventory.AssignItem(inventoryItem));

        return slot;
    }

    public void UpdateSlot(InventoryItem inventoryItem, int count)
    {
        itemToSlotMap[inventoryItem].UpdateSlotCount(count);
    }

    public void DestroySlot(InventoryItem item)
    {
        Destroy(itemToSlotMap[item].gameObject);
        itemToSlotMap.Remove(item);
    }
}