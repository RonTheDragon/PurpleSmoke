using UnityEngine;

public abstract class ChargeableMoveSet : PlayerCombatMoveSet
{
    protected PlayerAnimations _playerAnimations;
    protected PlayerCharging _playerCharging;

    public override void OnHeavyAttack()
    {
        _playerCharging.ActivateCharge();
    }

    public override void OnReleaseHeavyAttack()
    {
        _playerCharging.ResetCharge();
    }

    public override void ResetAttacks()
    {
        _playerCharging.ResetCharge();
    } 

}
    
