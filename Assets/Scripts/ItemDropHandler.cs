using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    public List<WeaponSlot> equipmentSlots;

    public void OnDrop(PointerEventData eventData)
    {
        foreach (var equipmentSlot in equipmentSlots)
        {
            /*var ray = GameManager.Instance.PlayerController._camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000))
            {
                var item = hit.transform.gameObject.GetComponent<InventorySlot>().SlotItem;
                Debug.Log("Hit an Item! " + item.itemName);
                equipmentSlot.AssignItemToWeaponSlot(item, hit.transform.gameObject);
            }*/
        }
    }
}