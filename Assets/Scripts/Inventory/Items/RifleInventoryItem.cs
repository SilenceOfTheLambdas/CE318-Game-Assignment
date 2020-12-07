using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Rifle Item")]
public class RifleInventoryItem : InventoryItem
{
    public override void AssignItemToPlayer(PlayerEquipmentController playerEquipment)
    {
        playerEquipment.AssignRifleItem(this);
    }
}