using UnityEngine;

public abstract class CombatMoveSet : MonoBehaviour
{
    protected PlayerCombatSystem _playerCombatSystem;
    protected float _castTimeLeft;
    public abstract void OnLightAttack();

    public abstract void OnReleaseLightAttack();

    public abstract void OnHeavyAttack();

    public abstract void OnReleaseHeavyAttack();

    public virtual void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
    }

    public abstract void MoveSetUpdate();

    public abstract void ResetAttacks();

    public virtual void SubscribeToEvents() { }
    public virtual void UnsubscribeToEvents() { }
}
