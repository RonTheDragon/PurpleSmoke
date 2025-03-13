using System;
using UnityEngine;

public class PlayerCharging : MonoBehaviour, IPlayerComponent
{
    public Action<float> OnChargeChange;
    private Action _onReleaseCharge; // Store release method (no parameters)
    //private float _currentChargePercentage;
    private PlayerAnimations _playerAnimations;
    private MonoBehaviour _classUsingCharge;

    [ReadOnly][SerializeField] protected float _maxCharge;
    [ReadOnly][SerializeField] protected float _minCharge;
    [ReadOnly][SerializeField] protected float _currentCharge;
    [ReadOnly][SerializeField] protected bool _isCharging;
    [ReadOnly][SerializeField] protected bool _releasedEarly;
    [ReadOnly][SerializeField] protected bool _releaseWhenFullyCharged;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        playerComponents.OnUpdate += PlayerUpdate;
        _playerAnimations = playerComponents.GetPlayerAnimations;
    }

    private void PlayerUpdate()
    {
        if (_releasedEarly && _currentCharge >= _minCharge)
        {
            _currentCharge = _minCharge; // Ensure minimum charge threshold
            _releasedEarly = false;
            ReleaseCharge();
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
                    ReleaseCharge();
            }
        }
    }

    private void DisplayChargeBar()
    {
        SetChargePercentage(_releasedEarly ? 0 : GetChargePercentage());
    }

    public float GetChargePercentage()
    {
        float n = (_currentCharge - _minCharge) / (_maxCharge - _minCharge);
        return Mathf.Clamp01(n);
    }

    public void SetChargePercentage(float charge)
    {
        //_currentChargePercentage = charge;
        OnChargeChange?.Invoke(charge);
    }

    public void ActivateCharge(MonoBehaviour classUsingCharge)
    {
        _classUsingCharge = classUsingCharge;
        _isCharging = true;
    }

    public void ResetCharge(MonoBehaviour classUsingCharge)
    {
        if (_classUsingCharge != classUsingCharge) return;
        ResetingCharge();
        _classUsingCharge = null;
    }

    public void ForceResetCharge()
    {
        ResetingCharge();
       
        if (_classUsingCharge is UseableAbility)
        {
            UseableAbility u = (UseableAbility)_classUsingCharge;
            u.OnCancel();
            
        }
        _classUsingCharge = null;
    }

    private void ResetingCharge()
    {
        _releasedEarly = false;
        _isCharging = false;
        _currentCharge = 0;
        SetChargePercentage(0);
        _onReleaseCharge = null; // Clear stored action
    }

    public bool CheckAndHandleEarlyRelease()
    {
        if (_currentCharge < _minCharge)
        {
            _releasedEarly = true;
        }
        return _releasedEarly;
    }

    public void PerformCharging(ChargeableStats chargeable, Action releaseMethod)
    {
        _releaseWhenFullyCharged = chargeable.ReleaseOnFull;
        _maxCharge = chargeable.MaxChargeTime;
        _minCharge = chargeable.MinChargeTime;
        _playerAnimations.PlayAnimation(chargeable.ChargeAnimationName);
        _onReleaseCharge = releaseMethod; // Store the method for releasing the attack
    }

    private void ReleaseCharge()
    {
        _onReleaseCharge?.Invoke(); // Call stored action (no parameters)
        ResetCharge(this);
    }

    public bool GetReleasedEarly => _releasedEarly;
    public bool GetIsCharging => _isCharging;
}


[System.Serializable]
public class ChargeableStats
{
    public string ChargeAnimationName;
    public float MinChargeTime, MaxChargeTime;
    public bool ReleaseOnFull;
}
