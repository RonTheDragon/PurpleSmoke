using System;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour, IPlayerComponent
{
    public Action<float> OnChargeChange;

    private CharacterController _controller;
    private PlayerGroundCheck _groundCheck;
    private PlayerGravity _gravity;
    private PlayerAnimations _animations;
    private PlayerWalk _movement;
    private PlayerAttackMovement _attackMovement;
    private PlayerGlide _glide;
    private bool _canAttack = true;

    private float _currentChargePercentage;

    [SerializeField] private CombatMoveSet _defaultMoveSet;
    [ReadOnly][SerializeField] private CombatMoveSet _currentMoveSet;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _controller = playerComponents.GetCharacterController();
        _groundCheck = playerComponents.GetPlayerGroundCheck();
        _animations = playerComponents.GetPlayerAnimations();
        _movement = playerComponents.GetPlayerWalk();
        _attackMovement = playerComponents.GetPlayerAttackMovement();
        _glide = playerComponents.GetPlayerGlide();
        _gravity = playerComponents.GetPlayerGravity();

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
    public PlayerWalk GetMovement()
    {
        return _movement;
    }
    public PlayerAttackMovement GetAttackMovement()
    {
        return _attackMovement;
    }

    public CombatMoveSet GetDefaultMoveSet()
    {
        return _defaultMoveSet;
    }

    public PlayerGravity GetGravity()
    {
        return _gravity;
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
        if (CanPlayerAttack())
            _currentMoveSet?.OnLightAttack();
    }

    public void OnReleaseLightAttack()
    {
        if (CanPlayerAttack())
            _currentMoveSet?.OnReleaseLightAttack();
    }

    public void OnHeavyAttack()
    {
        if (CanPlayerAttack())
            _currentMoveSet?.OnHeavyAttack();
    }

    public void OnReleaseHeavyAttack()
    {
        if (CanPlayerAttack())
            _currentMoveSet?.OnReleaseHeavyAttack();
    }

    private bool CanPlayerAttack()
    {
        return !_glide.IsGliding() && _canAttack;
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

    public void ClearAttacks()
    {
        _currentMoveSet.ResetAttacks();
    }

    public void SetCanAttack(bool canAttack)
    {
        _canAttack = canAttack;
    }

    public bool GetCanAttack() { return _canAttack; }
}
