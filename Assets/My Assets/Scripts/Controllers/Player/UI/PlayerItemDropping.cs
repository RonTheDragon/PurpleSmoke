using System;
using System.Linq;
using UnityEngine;

public class PlayerItemDropping : MonoBehaviour , IPlayerComponent
{
    private GameManager _gameManager;
    private PickupPooler _pickupPooler;
    private PlayerComponentsRefrences _playerComponents;
    private PlayerInventory _playerInventory;
    private Transform _playerBody;

    private bool _pressingDrop;
    private float _itemDropStackTimeLeft;

    [SerializeField] private Vector2 _itemDropVelocity;
    [SerializeField] private float _itemDropProtectionTime, _itemDropHeight, _itemDropStackTime;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _gameManager = GameManager.Instance;
        _pickupPooler = _gameManager.GetPickupPooler;
        _playerComponents = playerComponents;
        _playerInventory = _playerComponents.GetPlayerInventory;
        _playerBody = _playerComponents.GetPlayerBody;
    }

    public void PressDropItem()
    {
        _pressingDrop = true;
        if (!_playerComponents.OnUpdate.GetInvocationList().Contains((Action)ReleaseStackTimer))
        {
            _playerComponents.OnUpdate += ReleaseStackTimer;
        }
        _itemDropStackTimeLeft = _itemDropStackTime;
    }

    public void ReleaseDropItem()
    {
        if (_pressingDrop)
        {
            DropOne();
        }
        _pressingDrop = false;
        _playerComponents.OnUpdate -= ReleaseStackTimer;
    }

    private void ReleaseStackTimer()
    {
        if (_itemDropStackTimeLeft > 0)
        {
            _itemDropStackTimeLeft -= Time.deltaTime;
        }
        else
        {
            DropStack();
            _playerComponents.OnUpdate -= ReleaseStackTimer;
        }
    }

    private void DropOne()
    {
        if (_playerInventory.GetSelected)
        {
            ItemUI selectedItem = _playerInventory.GetSelected.GetComponent<ItemUI>();
            DropItem(selectedItem, 1, false); // Drop a single item
        }
    }

    private void DropStack()
    {
        if (_playerInventory.GetSelected)
        {
            ItemUI selectedItem = _playerInventory.GetSelected.GetComponent<ItemUI>();
            DropItem(selectedItem, selectedItem.GetAmount, true); // Drop the entire stack
        }
    }

    private void DropItem(ItemUI itemUI, int amount, bool isStack)
    {
        if (itemUI != null)
        {
            InventoryItem item = itemUI.GetInventoryItem;
            if (item.CanBeDropped)
            {
                if (isStack)
                {
                    _playerInventory.RemoveItemStack(itemUI); // Remove the entire stack
                }
                else
                {
                    _playerInventory.RemoveOneItem(itemUI); // Remove one item
                }

                // Spawn the item pickup from the pool
                ItemPickUp pickUp = (ItemPickUp)_pickupPooler.CreateOrSpawnFromPool(
                    item.GetPickUpTag,
                    _playerBody.position + Vector3.up * _itemDropHeight,
                    Quaternion.identity
                );

                // Set the item amount (1 for single item drop, or stack amount)
                pickUp.SetAmount(amount);

                // Apply item drop protection and add force
                pickUp.Spawn(_itemDropProtectionTime);
                pickUp.GetRigidbody.AddForce(
                    _playerBody.forward * _itemDropVelocity.x + Vector3.up * _itemDropVelocity.y
                );
            }
        }
    }
}
