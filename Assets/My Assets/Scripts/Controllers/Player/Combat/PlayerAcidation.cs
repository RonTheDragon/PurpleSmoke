using System;
using UnityEngine;

public class PlayerAcidation : MonoBehaviour, IPlayerComponent
{
    [Header("Acidation Meter")]
    [SerializeField] private float _maxAcidation = 100;
    [SerializeField][ReadOnly] private float _currentAcidation;
    [SerializeField] private float _minAcidationForUse = 20;
    [SerializeField] private float _acidationRegenPerSec = 40;
    [SerializeField] private float _acidationReganCooldown = 2;
    private float _acidationReganCooldownTimeLeft;
    [SerializeField] private float _acidationDrainPerSec = 10;

    [Header("Acidation Modifiers")]
    [SerializeField] private float _speedModifier = 1.5f;

    private bool _isAcidationActivated = false;
    private bool _canGenerateAcidation = true;

    public Action<float> OnAcidationChange;
    public Action<bool> OnAcidationToggle;

    private PlayerWalk _playerWalk;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerWalk = playerComponents.GetPlayerWalk();

        SetAcidationToMax();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        HandleAcidationMeter();
    }

    public void OnAcidationInputDown()
    {
        if (!_isAcidationActivated && _currentAcidation >= _minAcidationForUse)
        {
            TurnAcidationOn();
        }
        else if (_isAcidationActivated)
        {
            TurnAcidationOff();
        }
    }

    public void SetAcidationToMax()
    {
        _currentAcidation = _maxAcidation;
        UpdateAcidation();
    }

    public void SetAcidationToEmpty()
    {
        _currentAcidation = 0;
        UpdateAcidation();
    }

    private void HandleAcidationMeter()
    {
        if (_isAcidationActivated)
        {
            AcidationDraining();
        }
        else if (_canGenerateAcidation)
        {
            AcidationRegeneration();
        }
    }

    private void AcidationDraining()
    {
        if (_currentAcidation > 0)
        {
            _currentAcidation -= _acidationDrainPerSec * Time.deltaTime;
            UpdateAcidation();
        }
        else if (_currentAcidation < 0)
        {
            TurnAcidationOff();
        }
    }

    private void AcidationRegeneration()
    {
        if (_acidationReganCooldownTimeLeft > 0)
        {
            _acidationReganCooldownTimeLeft -= Time.deltaTime;
        }
        else if (_currentAcidation < _maxAcidation)
        {
            _currentAcidation += _acidationRegenPerSec * Time.deltaTime;
            UpdateAcidation();
        }
        else
        {
            _currentAcidation = _maxAcidation;
            UpdateAcidation();
        }
    }

    private void UpdateAcidation()
    {
        OnAcidationChange?.Invoke(_currentAcidation / _maxAcidation);
    }

    private void TurnAcidationOn()
    {
        _isAcidationActivated = true;
        _playerWalk.AddSpeedModifier("Acidation", _speedModifier);
        OnAcidationToggle?.Invoke(true);
    }

    private void TurnAcidationOff()
    {
        _isAcidationActivated = false;
        _playerWalk.RemoveSpeedModifier("Acidation");
        OnAcidationToggle?.Invoke(false);
        _acidationReganCooldownTimeLeft = _acidationReganCooldown;
    }

    public void SetCanGenerateAcidation(bool canGenerateAcidation)
    {
        _canGenerateAcidation = canGenerateAcidation;
    }
}
