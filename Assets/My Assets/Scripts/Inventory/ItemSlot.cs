using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    public Button GetButton => _button;

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
}
