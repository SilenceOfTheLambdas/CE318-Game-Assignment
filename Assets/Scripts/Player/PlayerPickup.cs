using System;
using Inventory;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerPickup : MonoBehaviour
    {
        // Reference to the main camera
        [SerializeField] private TextMeshProUGUI pickupText; // The text shown when hovering over an item
        public                   Camera          Camera;
        public                   float           PickUpRange;
        public                   GameObject      slotPrefab;
        public                   int             pickupLayerMask;

        private void Start()
        {
            pickupLayerMask = LayerMask.GetMask("Pickup");
        }

        private void Update()
        {

            var ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out var hit, PickUpRange, pickupLayerMask))
            {
                pickupText.gameObject.SetActive(true);
                if (Input.GetKeyDown(InputManager.Instance.PickupKey))
                {
                    var pickable = hit.transform.GetComponent<PickableItem>();
                    if (pickable)
                    {
                        PickItem(pickable);
                        Destroy(hit.transform.gameObject);
                        pickupText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                pickupText.gameObject.SetActive(false);
            }
        }

        private void PickItem(PickableItem item)
        {
            // Disable rigidbody
            /*item.Rb.isKinematic = true;
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;*/

            GetComponent<Inventory.Inventory>().AddItem(item.itemType, 1);
            slotPrefab.GetComponent<InventorySlot>().SlotItem = item.itemType;
        }
    }
}