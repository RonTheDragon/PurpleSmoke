using UnityEngine;

public abstract class PlayerCombatMoveSet : CombatMoveSet
{
    protected PlayerCombatSystem _playerCombatSystem;
    public abstract void OnLightAttack();

    public abstract void OnReleaseLightAttack();

    public abstract void OnHeavyAttack();

    public abstract void OnReleaseHeavyAttack();

    public override void MoveSetStart(CombatSystem playerCombatSystem)
    {
        _playerCombatSystem = (PlayerCombatSystem)playerCombatSystem;
    }
}
