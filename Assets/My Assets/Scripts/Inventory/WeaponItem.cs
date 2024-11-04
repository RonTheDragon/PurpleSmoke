using UnityEngine;

public abstract class WeaponItem : InventoryItem
{
    [SerializeField] private PlayerCombatMoveSet _moveSet;


    public PlayerCombatMoveSet GetMoveSet => _moveSet;
}
