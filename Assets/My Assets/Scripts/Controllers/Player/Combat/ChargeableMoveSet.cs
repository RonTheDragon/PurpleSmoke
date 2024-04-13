using UnityEngine;

public abstract class ChargeableMoveSet : CombatMoveSet
{
    protected PlayerAnimations _playerAnimations;

    [SerializeField] protected float _maxCharge;
    [SerializeField] protected float _minCharge;
    [ReadOnly][SerializeField] protected float _currentCharge;
    [ReadOnly][SerializeField] protected bool _isCharging;
    [ReadOnly][SerializeField] protected bool _releasedEarly;
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
        if (_releasedEarly && _currentCharge >= _minCharge)
        {
            _currentCharge = _minCharge; //just to make sure its minimum damage
            _releasedEarly = false;
            OnReleaseHeavyAttack();
        }

        if (_isCharging)
        {
            DisplayChargeBar();

            if (_currentCharge < _maxCharge)
            {
                _currentCharge += Time.deltaTime;
            }
            else if (_currentCharge > _maxCharge)
            {
                _currentCharge = _maxCharge;
                if (_releaseWhenFullyCharged)
                    OnReleaseHeavyAttack();
            }
        }
    }

    private void DisplayChargeBar()
    {
        if (_releasedEarly)
        {
            _playerCombatSystem.SetChargePercentage(0);
        }
        else
        {
            _playerCombatSystem.SetChargePercentage(GetChargePercentage());
        }
    }

    protected void ResetCharge()
    {
        _releasedEarly = false;
        _isCharging = false;
        _currentCharge = 0;
        _playerCombatSystem.SetChargePercentage(0);
    }

    protected bool CheckAndHandleEarlyRelease()
    {
        if (_currentCharge < _minCharge)
        {
            _releasedEarly = true;
        }
        return _releasedEarly;
    }

    public override void ResetAttacks()
    {
        ResetCharge();
    }

    public float GetChargePercentage()
    {
        float n = (_currentCharge - _minCharge) / (_maxCharge - _minCharge);
        return n < 0 ? 0 : n;
    }

    protected void PerformCharging(ChargeableStats chargeable)
    {
        _releaseWhenFullyCharged = chargeable.ReleaseOnFull;
        _maxCharge = chargeable.MaxChargeTime;
        _minCharge = chargeable.MinChargeTime;
        _playerAnimations.PlayAnimation(chargeable.ChargeAnimationName);
    }

}
    [System.Serializable]
    public class ChargeableStats
    {
        public string ChargeAnimationName;
        public float MaxChargeTime;
        public float MinChargeTime;
        public bool ReleaseOnFull;
    }
