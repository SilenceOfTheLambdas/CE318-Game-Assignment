namespace Inventory.Items
{
    public class PistolInventoryItem : InventoryItem
    {
        public override void AssignItemToPlayer(PlayerEquipmentController playerEquipment)
        {
            playerEquipment.AssignPistolItem(this);
        }
    }
}