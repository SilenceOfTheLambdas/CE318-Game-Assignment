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
                        Destroy(hit.transform.gameObject);
                    }
                }
            }

            var hoverRay = Camera.ViewportPointToRay(Vector3.one * 0.5f);
            if (!Physics.Raycast(hoverRay, out var hoverHit, PickUpRange)) return;
            
            if (hoverHit.transform.gameObject.GetComponent<PickableItem>() != null)
            {
                pickupText.gameObject.SetActive(true);
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