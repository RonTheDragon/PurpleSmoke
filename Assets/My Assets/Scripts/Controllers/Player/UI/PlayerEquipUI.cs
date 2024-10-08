using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipUI : MonoBehaviour
{
    [SerializeField] private EquipDisplayItemSlot _meleeSlot, _rangeSlot, _staticSlot, _dynamicSlot;
    [SerializeField] private List<ShortcutItemSlot> _keyboardShortcutsSlots;

    public EquipDisplayItemSlot GetMeleeSlot => _meleeSlot;
    public EquipDisplayItemSlot GetRangeSlot => _rangeSlot;
    public EquipDisplayItemSlot GetStaticSlot => _staticSlot;
    public EquipDisplayItemSlot GetDynamicSlot => _dynamicSlot;
    public List<ShortcutItemSlot> GetShortcutItemSlots => _keyboardShortcutsSlots;

    public void SetUp()
    {
        _meleeSlot.SetColor();
        _rangeSlot.SetColor();
        _staticSlot.SetColor();
        _dynamicSlot.SetColor();
    }

    public void SetUpShortcutsDisplay(List<InventoryItemUI> items)
    {
        _meleeSlot.SetColor();
        _rangeSlot.SetColor();
        _staticSlot.SetColor();
        _dynamicSlot.SetColor();
        
        for (int i = 0; i < 8; i++) 
        {
            _keyboardShortcutsSlots[i].SetShortcutKey(i+1);
            if (items.Count > i)
            {
                _keyboardShortcutsSlots[i].SetSlot(items[i]);
                _keyboardShortcutsSlots[i].SetColorUsingType(items[i].GetItemType);
            }
            else
            {
                _keyboardShortcutsSlots[i].ClearSlot();
                _keyboardShortcutsSlots[i].ClearColor();
            }
        }
    }
}
