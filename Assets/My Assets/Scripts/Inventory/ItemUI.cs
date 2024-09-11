using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private InventoryItem _inventoryItem;
    public enum ItemType { Melee, Range, Useable, Consumable}
    private ItemType _itemType;

    [SerializeField] private Image _itemImage, _backgroundImage;
    [SerializeField] private Color _meleeColor, _rangeColor, _useableColor, _consumableColor;

    public InventoryItem GetInventoryItem => _inventoryItem;

    public void SetUpItemUI(InventoryItem item)
    {
        _inventoryItem = item;
        _itemImage.sprite = _inventoryItem.GetSprite;
        SetItemType();
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
}
