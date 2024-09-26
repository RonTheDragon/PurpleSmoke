using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private InventoryItem _inventoryItem;
    public enum ItemType { Melee, Range, Useable, Consumable}
    private ItemType _itemType;
    private PlayerUI _playerUI;
    [SerializeField] private int _amount;

    [SerializeField] private Image _itemImage, _backgroundImage;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private Color _meleeColor, _rangeColor, _useableColor, _consumableColor;

    public InventoryItem GetInventoryItem => _inventoryItem;
    public ItemType GetItemType => _itemType;

    public void SetUpItemUI(InventoryItemWithAmount item,PlayerUI playerUI)
    {
        _inventoryItem = item.Item;
        _amount = item.Amount;
        _playerUI = playerUI;
        _itemImage.sprite = _inventoryItem.GetSprite;
        SetItemType();
        SetAmount();
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
            _playerUI.SetMeleeWeapon((MeleeItem)_inventoryItem);
        }
        else if (_itemType == ItemType.Range)
        {
            _playerUI.SetRangeWeapon((RangeItem)_inventoryItem);
        }
        else if (_itemType == ItemType.Useable)
        {
            _playerUI.SetStaticUseable((UseableItem)_inventoryItem);
        }
        else if (_itemType == ItemType.Consumable)
        {
            if (_playerUI.SpendConsumable((ConsumableItem)_inventoryItem))
            {
                RemoveOneItem();
                _playerUI.RemoveItem(this);
            }
        }
    }

    public void SetAmount()
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

    public void RemoveOneItem()
    {
        _amount--;
        SetAmount();
    }

    public int GetAmount => _amount;
}
