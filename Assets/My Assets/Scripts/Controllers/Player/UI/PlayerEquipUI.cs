using UnityEngine;

public class PlayerEquipUI : MonoBehaviour , IPlayerComponent
{
    [SerializeField] private EquipDisplayItemSlot _meleeSlot, _rangeSlot, _staticSlot, _dynamicSlot;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        
    }

    public EquipDisplayItemSlot GetMeleeSlot => _meleeSlot;
    public EquipDisplayItemSlot GetRangeSlot => _rangeSlot;
    public EquipDisplayItemSlot GetStaticSlot => _staticSlot;
    public EquipDisplayItemSlot GetDynamicSlot => _dynamicSlot;
}
