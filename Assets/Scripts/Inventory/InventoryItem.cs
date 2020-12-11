using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    /// <summary>
    ///     Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
    /// </summary>
    public abstract class InventoryItem : MonoBehaviour
    {
        [SerializeField] public  GameObject itemPrefab;
        [SerializeField] public  Sprite     itemSprite;
        [SerializeField] public  string     itemName;
        [SerializeField] private Vector3    itemLocalPosition;
        [SerializeField] private Vector3    itemLocalRotation;

        public Sprite GetSprite()
        {
            return itemSprite;
        }

        public string GetName()
        {
            return itemName;
        }

        public GameObject GetPrefab()
        {
            return itemPrefab;
        }

        public Vector3 GetLocalPosition()
        {
            return itemLocalPosition;
        }

        public Quaternion GetLocalRotation()
        {
            return Quaternion.Euler(itemLocalRotation);
        }

        public abstract void AssignItemToPlayer(PlayerEquipmentController playerEquipment);
    }
}