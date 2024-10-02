using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private InventoryItem _inventoryItem;
    public enum ItemType { Melee, Range, Useable, Consumable}
    private ItemType _itemType;
    private PlayerInventory _playerInventory;
    [SerializeField] private int _amount;

    [SerializeField] private Image _itemImage, _backgroundImage;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private Color _meleeColor, _rangeColor, _useableColor, _consumableColor;

    public InventoryItem GetInventoryItem => _inventoryItem;
    public ItemType GetItemType => _itemType;

    public void SetUpItemUI(PlayerInventory.InventoryItemWithAmount item,PlayerInventory playerInventory)
    {
        _inventoryItem = item.Item;
        _amount = item.Amount;
        _playerInventory = playerInventory;
        _itemImage.sprite = _inventoryItem.GetSprite;
        SetItemType();
        UpdateAmountText();
        _button.onClick.AddListener(QuickEquip);
    }

    private void SetItemType()
    {
        if (_inventoryItem is WeaponItem) 
        {
            if (_inventoryItem is MeleeItem)
            {
                _itemType = ItemType.Melee;
                _backgroundImage.color = _meleeColor;
            }
            else if (_inventoryItem is RangeItem)
            {
                _itemType = ItemType.Range;
                _backgroundImage.color = _rangeColor;
            }
        }
        else if (_inventoryItem is UseableItem)
        {
            _itemType = ItemType.Useable;
            _backgroundImage.color = _useableColor;
        }
        else if (_inventoryItem is ConsumableItem)
        {
            _itemType = ItemType.Consumable;
            _backgroundImage.color = _consumableColor;
        }
    }

    private void QuickEquip()
    {
        if (_itemType == ItemType.Melee)
        {
            _playerInventory.SetMeleeWeapon((MeleeItem)_inventoryItem);
        }
        else if (_itemType == ItemType.Range)
        {
            _playerInventory.SetRangeWeapon((RangeItem)_inventoryItem);
        }
        else if (_itemType == ItemType.Useable)
        {
            _playerInventory.SetStaticUseable((UseableItem)_inventoryItem);
        }
        else if (_itemType == ItemType.Consumable)
        {
            if (_playerInventory.SpendConsumable((ConsumableItem)_inventoryItem))
            {
                _playerInventory.RemoveOneItem(this);
            }
        }
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
