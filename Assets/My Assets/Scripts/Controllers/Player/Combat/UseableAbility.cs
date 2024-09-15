using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseableAbility : MonoBehaviour
{
    protected PlayerCombatSystem _playerCombatSystem;
    public abstract void OnPress();

    public abstract void OnRelease();

    public virtual void UseableStart(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
    }
}
