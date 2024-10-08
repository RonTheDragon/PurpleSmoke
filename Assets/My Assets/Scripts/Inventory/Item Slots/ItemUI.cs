using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemUI : MonoBehaviour
{
    [SerializeField] protected Image _itemImage;
    [SerializeField] protected Image _backgroundImage;
    [SerializeField] protected TMP_Text _amountText;
    private InventoryItemUI _currentItem;
    public enum ItemType { Melee, Range, Useable, Consumable }
    public void SetImage(Sprite sprite)
    {
        if (sprite == null)
        {
            _itemImage.gameObject.SetActive(false);
        }
        else
        {
            _itemImage.gameObject.SetActive(true);
            _itemImage.sprite = sprite;
        }
    }

    public void SetAmount(int amount) 
    {
        if (amount == 1)
        {
            _amountText.text = string.Empty;
        }
        else
        {
            _amountText.text = amount.ToString();
        }
    }

    public void SetSlot(InventoryItemUI item)
    {
        CancelSubcribeToPrevious();
        _currentItem = item;
        _currentItem.OnAmountChange += SetAmount;
        SetImage(_currentItem.GetInventoryItem.GetSprite);
        SetAmount(_currentItem.GetAmount);
    }

    public void ClearSlot()
    {
        CancelSubcribeToPrevious();
        _currentItem = null;
        SetImage(null);
        SetAmount(1);
    }

    private void CancelSubcribeToPrevious()
    {
        if (_currentItem != null)
        {
            _currentItem.OnAmountChange -= SetAmount;
        }
    }
}
