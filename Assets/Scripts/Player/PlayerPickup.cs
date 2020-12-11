using Inventory;
using Inventory.Items;
using UnityEngine;

namespace Player
{
    public class PlayerPickup : MonoBehaviour
    {
        // Reference to the main camera
        public Camera     Camera;
        public float      PickUpRange;
        public Sprite     test;
        public GameObject itemPrefab;
        public GameObject slotPrefab;

        // The picked item
        private PickableItem _pickedItem;

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                var ray = Camera.ViewportPointToRay(Vector3.one * 0.5f);
                if (Physics.Raycast(ray, out var hit, PickUpRange))
                {
                    var pickable = hit.transform.GetComponent<PickableItem>();

                    if (pickable)
                    {
                        PickItem(pickable);
                    }
                }
            }
        }

        private void PickItem(PickableItem item)
        {
            _pickedItem = item;
        
            // Disable rigidbody
            item.Rb.isKinematic = true;
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;

            var _item = item.GetComponent<RifleInventoryItem>();
            _item.name = "AKM";
            _item.itemSprite = test;
            _item.itemPrefab = itemPrefab;
            _item.WeaponType = RifleInventoryItem.WeaponTypes.Primary;
            
            GetComponent<Inventory.Inventory>().AddItem(_item, 1);
            GetComponent<PlayerEquipmentController>().AssignRifleItem(_item);
            
            slotPrefab.GetComponent<InventorySlot>().SlotItem = _item;
        }
    }
}
