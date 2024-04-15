using System;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour, IPlayerComponent
{
    public Action<float> OnChargeChange;

    private CharacterController _controller;
    private PlayerGroundCheck _groundCheck;
    private PlayerGravity _gravity;
    private PlayerJump _playerJump;
    private PlayerAnimations _animations;
    private PlayerWalk _movement;
    private PlayerAttackMovement _attackMovement;
    private PlayerGlide _glide;
    private PlayerAcidation _playerAcidation;
    private bool _canAttack = true;
    private bool _acidation = false;
    private bool _usingRanged;
    private Transform _shooter;
    private bool _busyAttacking;

    private float _currentChargePercentage;

    [SerializeField] private CombatMoveSet _defaultMeleeMoveSet;
    [ReadOnly][SerializeField] private CombatMoveSet _currentMeleeMoveSet;
    [SerializeField] private CombatMoveSet _defaultRangeMoveSet;
    [ReadOnly][SerializeField] private CombatMoveSet _currentRangeMoveSet;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _controller = playerComponents.GetCharacterController();
        _groundCheck = playerComponents.GetPlayerGroundCheck();
        _animations = playerComponents.GetPlayerAnimations();
        _movement = playerComponents.GetPlayerWalk();
        _attackMovement = playerComponents.GetPlayerAttackMovement();
        _glide = playerComponents.GetPlayerGlide();
        _gravity = playerComponents.GetPlayerGravity();
        _playerAcidation = playerComponents.GetPlayerAcidation();
        _shooter = playerComponents.GetShooter();
        _playerJump = playerComponents.GetPlayerJump();

        TemporaryStart();
        _glide.OnGlide += ClearAttacks;
        playerComponents.OnUpdate += PlayerUpdate;
        _playerAcidation.OnAcidationToggle += SetAcidation;
    }

    private void TemporaryStart()
    {
        _currentMeleeMoveSet = _defaultMeleeMoveSet;
        _currentMeleeMoveSet.MoveSetStart(this);
        _currentRangeMoveSet = _defaultRangeMoveSet;
        _currentRangeMoveSet.MoveSetStart(this);
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
        return _defaultMeleeMoveSet;
    }

    public PlayerGravity GetGravity()
    {
        return _gravity;
    }

    public PlayerJump GetJump()
    {
        return _playerJump;
    }

    private void PlayerUpdate()
    {
        _defaultMeleeMoveSet.MoveSetUpdate();
        if (_currentMeleeMoveSet != _defaultMeleeMoveSet)
        {
            _currentMeleeMoveSet?.MoveSetUpdate();
        }

        _defaultRangeMoveSet.MoveSetUpdate();
        if (_currentRangeMoveSet != _defaultRangeMoveSet)
        {
            _currentRangeMoveSet?.MoveSetUpdate();
        }
    }

    public void OnLightAttack()
    {
        if (CanPlayerAttack())
        {
            if (_usingRanged)
            {
                _currentRangeMoveSet?.OnLightAttack();
            }
            else
            {
                _currentMeleeMoveSet?.OnLightAttack();
            }
        }
    }

    public void OnReleaseLightAttack()
    {
        if (CanPlayerAttack())
        {
            if (_usingRanged)
            {
                _currentRangeMoveSet?.OnReleaseLightAttack();
            }
            else
            {
                _currentMeleeMoveSet?.OnReleaseLightAttack();
            }
        }
    }

    public void OnHeavyAttack()
    {
        if (CanPlayerAttack())
        {
            if (_usingRanged)
            {
                _currentRangeMoveSet?.OnHeavyAttack();
            }
            else
            {
                _currentMeleeMoveSet?.OnHeavyAttack();
            }
        }
    }

    public void OnReleaseHeavyAttack()
    {
        if (CanPlayerAttack())
        {
            if (_usingRanged)
            {
                _currentRangeMoveSet?.OnReleaseHeavyAttack();
            }
            else
            {
                _currentMeleeMoveSet?.OnReleaseHeavyAttack();
            }
        }
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
        _currentMeleeMoveSet.ResetAttacks();
        _currentRangeMoveSet.ResetAttacks();
    }

    public void SetCanAttack(bool canAttack)
    {
        _canAttack = canAttack;
    }

    public bool GetCanAttack() { return _canAttack; }

    public bool GetAcidation() { return _acidation; }

    public void SetAcidation(bool acidation)
    {
        _acidation = acidation;
    }
    public PlayerAcidation GetPlayerAcidation() { return _playerAcidation; }


    public void SetUsingRanged(bool usingRanged)
    {
        _usingRanged = usingRanged;
        ClearAttacks();
    }

    public Transform GetShooter()
    {
        return _shooter;
    }

    public void SetBusyAttacking(bool busyAttacking)
    {
        _busyAttacking = busyAttacking;
    }

    public bool GetIsBusyAttacking()
    {
        return _busyAttacking;
    }
}
