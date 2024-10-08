using UnityEngine;

public class EquipDisplayItemSlot : ItemUI
{
    [SerializeField] private GameManager.ItemColor colorType;

    public void SetColor()
    {
        GameManager gameManager = GameManager.Instance;
        _backgroundImage.color = gameManager.GetItemColor(colorType);
    }
}
