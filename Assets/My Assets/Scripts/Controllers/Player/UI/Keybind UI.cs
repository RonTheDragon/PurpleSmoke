using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindUI : MonoBehaviour
{
    [SerializeField] private string _inputActionName;
    [SerializeField] private Image _sprite;
    [SerializeField] private TMP_Text _keyText;

    public string GetInputActionName => _inputActionName;

    public void SetImage (Sprite sprite)
    {
        _keyText.gameObject.SetActive(false);
        _sprite.gameObject.SetActive(true);
        _sprite.sprite = sprite;
    }

    public void SetText(string text)
    {
        _keyText.gameObject.SetActive(true);
        _sprite.gameObject.SetActive(false);
        _keyText.text = text;
    }
}
