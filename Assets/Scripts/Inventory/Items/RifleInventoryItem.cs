namespace Inventory.Items
{
    public class RifleInventoryItem : InventoryItem
    {
        public override void AssignItemToPlayer(PlayerEquipmentController playerEquipment)
        {
            playerEquipment.AssignRifleItem(this);
        }
    }
}