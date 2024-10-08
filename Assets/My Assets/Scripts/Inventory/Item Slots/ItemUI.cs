using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemUI : MonoBehaviour
{
    [SerializeField] protected Image _itemImage;
    [SerializeField] protected Image _backgroundImage;
    [SerializeField] protected TMP_Text _amountText;
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

    public void SetSlot(Sprite image,int amount)
    {
        SetImage(image);
        SetAmount(amount);
    }

    public void ClearSlot()
    {
        SetImage(null);
        SetAmount(1);
    }
}
