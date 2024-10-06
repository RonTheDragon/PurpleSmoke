using UnityEngine;

public class PlayerEquipUI : MonoBehaviour , IPlayerComponent
{
    [SerializeField] private ItemSlot _meleeSlot, _rangeSlot, _staticSlot, _dynamicSlot;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        
    }

    public ItemSlot GetMeleeSlot => _meleeSlot;
    public ItemSlot GetRangeSlot => _rangeSlot;
    public ItemSlot GetStaticSlot => _staticSlot;
    public ItemSlot GetDynamicSlot => _dynamicSlot;
}
