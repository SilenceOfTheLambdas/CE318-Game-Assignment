using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class WeaponSlot : MonoBehaviour, IDropHandler
    {
        public enum SlotTypes
        {
            Primary,
            Secondary,
            Pistol
        }

        public                   SlotTypes     slotType;
        [HideInInspector]
        public                   GameObject    slot;
        [HideInInspector]
        public                   InventoryItem item; // The item the player has slotted into this slot
        [SerializeField] private Image         itemSpriteHolder; // The gameObject where the sprite is placed

        /// <summary>
        /// Assign an item to a weapon slot
        /// </summary>
        public void AssignItemToWeaponSlot(InventoryItem itemToAdd, GameObject pSlot)
        {
            // Set properties
            item = itemToAdd;
            itemSpriteHolder.gameObject.SetActive(true);
            itemSpriteHolder.sprite = item.itemSprite;
            pSlot.transform.position = transform.position;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                foreach (var itemsToMap in GameManager.Instance.inventoryUI.itemToSlotMap)
                {
                    if (itemsToMap.Value == eventData.pointerDrag.gameObject.GetComponent<InventorySlot>())
                    {
                        AssignItemToWeaponSlot(itemsToMap.Key, eventData.pointerDrag.gameObject);
                    }
                }
            }
        }
    }
}
