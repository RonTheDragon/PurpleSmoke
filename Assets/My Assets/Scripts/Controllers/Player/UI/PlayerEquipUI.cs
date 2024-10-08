using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipUI : MonoBehaviour
{
    [SerializeField] private EquipDisplayItemSlot _meleeSlot, _rangeSlot, _staticSlot, _dynamicSlot;
    [SerializeField] private List<ShortcutItemSlot> _keyboardShortcutsSlots, _controllerShortcutSlots;

    [SerializeField] private GameObject _keyboardPanel, _controllerPanel;

    [SerializeField] private CanvasGroup _shortcuts1, _shortcuts2;
    [SerializeField] private float _alphaOfTurnedOffShortcuts;
    private PlayerInputsHandler _inputHandler;

    public EquipDisplayItemSlot GetMeleeSlot => _meleeSlot;
    public EquipDisplayItemSlot GetRangeSlot => _rangeSlot;
    public EquipDisplayItemSlot GetStaticSlot => _staticSlot;
    public EquipDisplayItemSlot GetDynamicSlot => _dynamicSlot;

    private bool UsingController;

    private List<ShortcutItemSlot> _keysForShortcutsSlots;

    private bool _controllerSwitchedShortcuts;


    public bool IsSwitchedShortcuts => _controllerSwitchedShortcuts;
    public void Initialize(PlayerComponentsRefrences playerComponents) 
    {
        SetColorsToSlots();
        UsingController = playerComponents.GetPlayerInputsHandler.UsingController;
        SetInputMethodPanel();
    }

    private void SetColorsToSlots()
    {
        _meleeSlot.SetColor();
        _rangeSlot.SetColor();
        _staticSlot.SetColor();
        _dynamicSlot.SetColor();
    }

    private void SetInputMethodPanel()
    {
        if (UsingController)
        {
            _controllerPanel.SetActive(true);
            _keyboardPanel.SetActive(false);
            _keysForShortcutsSlots = _controllerShortcutSlots;
        }
        else
        {
            _controllerPanel.SetActive(false);
            _keyboardPanel.SetActive(true);
            _keysForShortcutsSlots = _keyboardShortcutsSlots;
        }
    }


    public void SetUpShortcutsDisplay(List<InventoryItemUI> items)
    {
         for (int i = 0; i < 8; i++)
         {
             _keysForShortcutsSlots[i].SetShortcutKey(i + 1);
             if (items.Count > i)
             {
                 _keysForShortcutsSlots[i].SetSlot(items[i]);
                 _keysForShortcutsSlots[i].SetColorUsingType(items[i].GetItemType);
             }
             else
             {
                 _keysForShortcutsSlots[i].ClearSlot();
                 _keysForShortcutsSlots[i].ClearColor();
             }
         }
    } 

    public void SwitchShortcuts()
    {
        _controllerSwitchedShortcuts = !_controllerSwitchedShortcuts;
        if (_controllerSwitchedShortcuts)
        {
            _shortcuts1.alpha = _alphaOfTurnedOffShortcuts;
            _shortcuts2.alpha = 1f;
        }
        else
        {
            _shortcuts1.alpha = 1f;
            _shortcuts2.alpha = _alphaOfTurnedOffShortcuts;
        }
    }
}
