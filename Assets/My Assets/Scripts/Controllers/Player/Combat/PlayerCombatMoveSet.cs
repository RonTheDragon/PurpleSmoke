using UnityEngine;

public abstract class PlayerCombatMoveSet : CombatMoveSet
{
    protected PlayerCombatSystem _playerCombatSystem;
    protected bool _isEquipped;
    public abstract void OnLightAttack();

    public abstract void OnReleaseLightAttack();

    public abstract void OnHeavyAttack();

    public abstract void OnReleaseHeavyAttack();
    public void SetEquippedState(bool equip)
    {
        // If the current state matches the requested state, skip further processing
        if (_isEquipped == equip)
            return;

        // Update the equipped state
        _isEquipped = equip;

        if (equip)
        {
            OnEquip();
        }
        else
        {
            OnUnequip();
        }
    }
    protected abstract void OnEquip();
    protected abstract void OnUnequip();

    public override void MoveSetStart(CombatSystem playerCombatSystem)
    {
        _playerCombatSystem = (PlayerCombatSystem)playerCombatSystem;
    }
}
