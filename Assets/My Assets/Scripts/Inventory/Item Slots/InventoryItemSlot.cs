using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : EquipDisplayItemSlot
{
    [SerializeField] private Button _button;
    public Button GetButton => _button;
}
