using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemSlot : MonoBehaviour
{
    [SerializeField] protected Image _image;
    [SerializeField] protected TMP_Text _amountText;
    public void SetImage(Sprite sprite)
    {
        if (sprite == null)
        {
            _image.gameObject.SetActive(false);
        }
        else
        {
            _image.gameObject.SetActive(true);
            _image.sprite = sprite;
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
