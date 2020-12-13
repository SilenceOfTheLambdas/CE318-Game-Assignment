using UnityEngine;

namespace Inventory
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickableItem : MonoBehaviour
    {
        public InventoryItem itemType;
        public Rigidbody Rb { get; private set; }

        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
        }
    }
}