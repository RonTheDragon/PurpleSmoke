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
        _controller = playerComponents.GetCharacterController;
        _groundCheck = playerComponents.GetPlayerGroundCheck;
        _animations = playerComponents.GetPlayerAnimations;
        _movement = playerComponents.GetPlayerWalk;
        _attackMovement = playerComponents.GetPlayerAttackMovement;
        _glide = playerComponents.GetPlayerGlide;
        _gravity = playerComponents.GetPlayerGravity;
        _playerAcidation = playerComponents.GetPlayerAcidation;
        _shooter = playerComponents.GetShooter;
        _playerJump = playerComponents.GetPlayerJump;

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

        HandleSubscribtion();
    }

    public CharacterController GetController => _controller;
    public PlayerGroundCheck GetGroundCheck => _groundCheck;
    public PlayerAnimations GetAnimations => _animations;
    public PlayerWalk GetMovement => _movement;
    public PlayerAttackMovement GetAttackMovement => _attackMovement;
    public CombatMoveSet GetDefaultMoveSet => _defaultMeleeMoveSet;
    public CombatMoveSet GetCurrentMeleeMoveSet => _currentMeleeMoveSet;
    public CombatMoveSet GetCurrentRangeMoveSet => _currentRangeMoveSet;
    public PlayerGravity GetGravity => _gravity;
    public PlayerJump GetJump => _playerJump;
    public Transform GetShooter => _shooter;
    public bool GetIsBusyAttacking => _busyAttacking;
    public bool GetCanAttack => _canAttack;
    public bool GetAcidation => _acidation;
    public PlayerGlide GetGlide => _glide;
    public PlayerAcidation GetPlayerAcidation => _playerAcidation;
    private bool CanPlayerAttack => !_glide.IsGliding() && _canAttack;
    public float GetChargePercentage => _currentChargePercentage;





    private void PlayerUpdate()
    {
        if (!_usingRanged)
        {
            _currentMeleeMoveSet?.MoveSetUpdate();
        }
        else
        {
            _currentRangeMoveSet?.MoveSetUpdate();
        }
    }

    public void OnLightAttack()
    {
        if (CanPlayerAttack)
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
        if (CanPlayerAttack)
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
        if (CanPlayerAttack)
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
        if (CanPlayerAttack)
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


    public void ClearAttacks()
    {
        if (!_usingRanged)
        {
            _currentMeleeMoveSet.ResetAttacks();
        }
        else
        {
            _currentRangeMoveSet.ResetAttacks();
        }
    }

    public void SetCanAttack(bool canAttack)
    {
        _canAttack = canAttack;
    }

    public void SetAcidation(bool acidation)
    {
        _acidation = acidation;
    }
    public void SetBusyAttacking(bool busyAttacking)
    {
        _busyAttacking = busyAttacking;
    }
    public void SetChargePercentage(float charge)
    {
        _currentChargePercentage = charge;
        OnChargeChange?.Invoke(charge);
    }

    public void SetUsingRanged(bool usingRanged)
    {
        ClearAttacks();
        _usingRanged = usingRanged;
        ClearAttacks();
        HandleSubscribtion();
    }

    private void SetMoveSet(ref CombatMoveSet currentMoveSet, CombatMoveSet newMoveSet, CombatMoveSet defaultMoveSet)
    {
        if (currentMoveSet != defaultMoveSet)
        {
            Destroy(currentMoveSet.gameObject);
        }
        if (newMoveSet == null)
        {
            newMoveSet = defaultMoveSet;
        }
        else
        {
            string n = newMoveSet.gameObject.name;
            newMoveSet = Instantiate(newMoveSet, transform.position, Quaternion.identity, transform);
            newMoveSet.gameObject.name = n;
        }
        ClearAttacks();
        currentMoveSet = newMoveSet;
        currentMoveSet.MoveSetStart(this);
        ClearAttacks();
        HandleSubscribtion();
    }

    public void SetMeleeMoveSet(CombatMoveSet meleeMoveSet)
    {
        SetMoveSet(ref _currentMeleeMoveSet, meleeMoveSet, _defaultMeleeMoveSet);
    }

    public void SetRangeMoveSet(CombatMoveSet rangeMoveSet)
    {
        SetMoveSet(ref _currentRangeMoveSet, rangeMoveSet, _defaultRangeMoveSet);
    }


    private void HandleSubscribtion()
    {
        if (_usingRanged)
        {
            _currentRangeMoveSet.SubscribeToEvents();
            _currentMeleeMoveSet.UnsubscribeToEvents();
        }
        else
        {
            _currentRangeMoveSet.UnsubscribeToEvents();
            _currentMeleeMoveSet.SubscribeToEvents();
        }
    }


}
