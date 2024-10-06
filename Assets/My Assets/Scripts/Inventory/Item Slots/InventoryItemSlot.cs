using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : ItemSlot
{
    [SerializeField] private Button _button;
    public Button GetButton => _button;
}
