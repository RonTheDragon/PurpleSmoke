using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IPlayerComponent
{
    private PlayerInventory _playerInventory;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerInventory = playerComponents.GetPlayerInventory;
    }

    public void PickUpItem(InventoryItem inventoryItem, int amount = 1)
    {
        _playerInventory.AddInventoryItem(inventoryItem, amount);
    }
}
