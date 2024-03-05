using UnityEngine;

public abstract class CombatMoveSet : MonoBehaviour
{
    protected PlayerCombatSystem _playerCombatSystem;
    public abstract void OnLightAttack();

    public abstract void OnReleaseLightAttack();

    public abstract void OnHeavyAttack();

    public abstract void OnReleaseHeavyAttack();

    public virtual void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
    }

    public abstract void MoveSetUpdate();
}
