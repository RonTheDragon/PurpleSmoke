using System;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour, IPlayerComponent
{
    public Action<float> OnChargeChange;

    private CharacterController _controller;
    private PlayerGroundCheck _groundCheck;
    private PlayerAnimations _animations;

    private float _currentChargePercentage;

    [SerializeField] private CombatMoveSet _defaultMoveSet;
    [ReadOnly][SerializeField] private CombatMoveSet _currentMoveSet;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _controller = playerComponents.GetCharacterController();
        _groundCheck = playerComponents.GetPlayerGroundCheck();
        _animations = playerComponents.GetPlayerAnimations();

        TemporaryStart();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void TemporaryStart()
    {
        _currentMoveSet = _defaultMoveSet;
        _currentMoveSet.MoveSetStart(this);
    }

    public CharacterController GetController()
    {
        return _controller;
    }
    public PlayerGroundCheck GetGroundCheck()
    {
        return _groundCheck;
    }
    public PlayerAnimations GetAnimations()
    {
        return _animations;
    }

    public CombatMoveSet GetDefaultMoveSet()
    {
        return _defaultMoveSet;
    }

    private void PlayerUpdate()
    {
        _defaultMoveSet.MoveSetUpdate();
        if (_currentMoveSet != _defaultMoveSet)
        {
            _currentMoveSet?.MoveSetUpdate();
        }
    }

    public void OnLightAttack()
    {
        _currentMoveSet?.OnLightAttack();
    }

    public void OnReleaseLightAttack()
    {
        _currentMoveSet?.OnReleaseLightAttack();
    }

    public void OnHeavyAttack()
    {
        _currentMoveSet?.OnHeavyAttack();
    }

    public void OnReleaseHeavyAttack()
    {
        _currentMoveSet?.OnReleaseHeavyAttack();
    }

    public void SetChargePercentage(float charge)
    {
        _currentChargePercentage = charge;
        OnChargeChange?.Invoke(charge);
    }

    public float GetChargePercentage()
    {
        return _currentChargePercentage;
    }
}
