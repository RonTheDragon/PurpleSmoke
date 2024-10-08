using TMPro;
using UnityEngine;

public abstract class ShortcutItem : ItemUI
{
    [SerializeField] protected GameObject _shortcutKeyBackground;
    [SerializeField] protected TMP_Text _shortcutKeyText;

    public void SetShortcutKey(int key)
    {
        if (key == 0)
        {
            _shortcutKeyBackground.SetActive(false);
        }
        else
        {
            _shortcutKeyBackground.SetActive(true);
            _shortcutKeyText.text = key.ToString();
        }
    }
}
