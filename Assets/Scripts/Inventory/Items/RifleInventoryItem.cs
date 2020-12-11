namespace Inventory.Items
{
    public class RifleInventoryItem : InventoryItem
    {
        public enum WeaponTypes
        {
            Primary,
            Secondary,
            Pistol
        }

        public WeaponTypes WeaponType;
        
        public override void AssignItemToPlayer(PlayerEquipmentController playerEquipment)
        {
            playerEquipment.AssignRifleItem(this);
        }
    }
}