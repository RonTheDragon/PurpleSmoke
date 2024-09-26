using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IPlayerComponent
{
    private PlayerUI _playerUI;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerUI = playerComponents.GetPlayerUI;
    }

    public void PickUpItem(InventoryItem inventoryItem, int amount = 1)
    {
        _playerUI.AddInventoryItem(inventoryItem, amount);
    }
}
