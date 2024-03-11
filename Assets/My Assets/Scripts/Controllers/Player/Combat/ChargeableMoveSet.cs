using UnityEngine;

public abstract class ChargeableMoveSet : CombatMoveSet
{
    [SerializeField] protected float _maxCharge;
    [ReadOnly][SerializeField] protected float _currentCharge;
    protected bool _isCharging;
    [SerializeField] protected bool _releaseWhenFullyCharged;

    public override void OnHeavyAttack()
    {
        _isCharging = true;
    }

    public override void OnReleaseHeavyAttack()
    {
        ResetCharge();
    }

    public override void MoveSetUpdate()
    {
        if (_isCharging)
        {
            _playerCombatSystem.SetChargePercentage(GetChargePercentage());
            if (_currentCharge < _maxCharge)
            { 
                _currentCharge += Time.deltaTime;
            }
            else if (_releaseWhenFullyCharged)
            {
                OnReleaseHeavyAttack();
            }
        }
    }

    protected void ResetCharge()
    {
        _isCharging = false;
        _currentCharge = 0;
        _playerCombatSystem.SetChargePercentage(0);
    }

    public void ResetAttacks()
    {
        ResetCharge();
    }

    public float GetChargePercentage()
    {
        return _currentCharge / _maxCharge;
    }

}
