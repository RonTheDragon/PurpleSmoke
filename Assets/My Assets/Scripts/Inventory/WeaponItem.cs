using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponItem : InventoryItem
{
    [SerializeField] private CombatMoveSet _moveSet;


    public CombatMoveSet GetMoveSet => _moveSet;
}
