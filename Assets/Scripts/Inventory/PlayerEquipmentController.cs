using Inventory.Items;
using Player;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    ///     Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
    ///     Adjusted by 1806094
    /// </summary>
    public class PlayerEquipmentController : MonoBehaviour
    {
        private static readonly  int              AkmEquip = Animator.StringToHash("akmEquip");
        [SerializeField] private Inventory        inventory;
        [SerializeField] private Transform        inventoryUIParent;
        [SerializeField] private PlayerController Player;

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
            Player.primaryWeapon = _currentEquippedWeapon;

            // Play Rifle equip animation
            GetComponent<Animator>().SetBool(AkmEquip, true);
        }
        
        public void AssignPistolItem(PistolInventoryItem pistolInventoryItem)
        {
            DestroyIfNotNull(_currentEquippedWeapon);
            _currentEquippedWeapon = CreateNewItemInstance(pistolInventoryItem, weaponAnchor);
            Player.thirdWeapon = _currentEquippedWeapon;

            // Play Rifle equip animation
            //GetComponent<Animator>().SetBool(AkmEquip, true);
        }

        private static GameObject CreateNewItemInstance(InventoryItem item, Transform anchor)
        {
            var itemInstance = Instantiate(item.GetPrefab(), anchor);
            itemInstance.transform.position = item.itemPrefab.transform.position;
            itemInstance.transform.rotation = item.itemPrefab.transform.rotation;
            return itemInstance;
        }

        private static void DestroyIfNotNull(GameObject obj)
        {
            if (obj)
                Destroy(obj);
        }

        public Transform GetUIParent()
        {
            return inventoryUIParent;
        }
    }
}