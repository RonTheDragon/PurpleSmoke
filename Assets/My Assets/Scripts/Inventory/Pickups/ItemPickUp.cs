using UnityEngine;

public class ItemPickUp : Pickup
{
    [SerializeField] private InventoryItem _inventoryItem;
    [SerializeField] private int _amount = 1;
    public override bool Interact(PlayerInteraction playerIntercation)
    {
        if (_inventoryItem == null) return false;

        playerIntercation.PickUpItem(_inventoryItem,_amount);
        DisableItem();

        return true;
    }

    public void SetAmount(int amount) => _amount = amount;
}
