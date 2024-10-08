using UnityEngine;

public class ShortcutItemSlot : ShortcutItem
{
    [SerializeField] protected Color _clearColor;
    private GameManager _gameManager;

    public void SetColorUsingType(ItemType type) 
    {
        if (_gameManager==null)
        _gameManager = GameManager.Instance;

        switch (type) 
        {
            case ItemType.Melee: _backgroundImage.color = _gameManager.GetItemColor(GameManager.ItemColor.Melee); break;
            case ItemType.Range: _backgroundImage.color = _gameManager.GetItemColor(GameManager.ItemColor.Range); break;
            case ItemType.Useable: _backgroundImage.color = _gameManager.GetItemColor(GameManager.ItemColor.Dynamic); break;
            case ItemType.Consumable: _backgroundImage.color = _gameManager.GetItemColor(GameManager.ItemColor.Consumable); break;
        }
    }

    public void ClearColor()
    {
        _backgroundImage.color = _clearColor;
    }
}
