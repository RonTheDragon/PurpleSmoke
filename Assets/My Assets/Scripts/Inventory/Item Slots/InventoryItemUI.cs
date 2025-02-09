using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : ShortcutItem
{
    private InventoryItem _inventoryItem;
    private ItemType _itemType;
    private PlayerInventory _playerInventory;
    private int _amount;

    [SerializeField] private Button _button;
    [SerializeField] private GameObject _switchPlaceIcon;

    public Action<int> OnAmountChange;

    public InventoryItem GetInventoryItem => _inventoryItem;
    public ItemType GetItemType => _itemType;

    private GameManager _gm;

    public void SetUpItemUI(PlayerInventory.InventoryItemWithAmount item,PlayerInventory playerInventory)
    {
        _gm = GameManager.Instance;
        _inventoryItem = item.Item;
        _amount = item.Amount;
        _playerInventory = playerInventory;
        _itemImage.sprite = _inventoryItem.GetSprite;
        SetItemType();
        UpdateAmountText();
        _button.onClick.AddListener(UseItem);
    }


    private void SetItemType()
    {
        if (_inventoryItem is WeaponItem) 
        {
            if (_inventoryItem is MeleeItem)
            {
                _itemType = ItemType.Melee;
                _backgroundImage.color = _gm.GetItemColor(GameManager.ItemColor.Melee);
            }
            else if (_inventoryItem is RangeItem)
            {
                _itemType = ItemType.Range;
                _backgroundImage.color = _gm.GetItemColor(GameManager.ItemColor.Range); 
            }
        }
        else if (_inventoryItem is UseableItem)
        {
            _itemType = ItemType.Useable;
            _backgroundImage.color = _gm.GetItemColor(GameManager.ItemColor.Static);
        }
        else if (_inventoryItem is ConsumableItem)
        {
            _itemType = ItemType.Consumable;
            _backgroundImage.color = _gm.GetItemColor(GameManager.ItemColor.Consumable);
        }
    }

    public void UseItem()
    {
        if (_itemType == ItemType.Melee)
        {
            _playerInventory.SetMeleeWeapon((MeleeItem)_inventoryItem,this);
        }
        else if (_itemType == ItemType.Range)
        {
            _playerInventory.SetRangeWeapon((RangeItem)_inventoryItem, this);
        }
        else if (_itemType == ItemType.Useable)
        {
            if (_playerInventory.IsInventoryOpen)
            _playerInventory.SetStaticUseable((UseableItem)_inventoryItem, this);
            else 
            _playerInventory.SetDynamicUseable((UseableItem)_inventoryItem, this);
        }
        else if (_itemType == ItemType.Consumable)
        {
            if (_playerInventory.SpendConsumable((ConsumableItem)_inventoryItem))
            {
                _playerInventory.RemoveOneItem(this);
                _playerInventory.RemoveItemIfEmpty(this);
            }
        }
    }

    public void SetSwitchPlaceUI(bool Active)
    {
        _switchPlaceIcon.SetActive(Active);
    }

    public void UpdateAmountText()
    {
        if (_amount == 1)
        {
            _amountText.text = "";
        }
        else
        {
            _amountText.text = _amount.ToString();
        }
        OnAmountChange?.Invoke(_amount);
    }

    public void AddAmountToItem(int amount = 1)
    {
        _amount += amount;
        UpdateAmountText();
    }

    public void RemoveAmountFromItem(int amount = 1)
    {
        _amount-= amount;
        UpdateAmountText();
    }

    public int GetAmount => _amount;
}
