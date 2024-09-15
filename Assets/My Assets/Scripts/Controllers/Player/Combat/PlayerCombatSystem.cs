using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour, IPlayerComponent
{
    public Action<float> OnChargeChange;

    private PlayerComponentsRefrences _playerComponentsRefrences;
    private PlayerGlide _glide;
    private PlayerAcidation _playerAcidation;
    private bool _canAttack = true;
    private bool _acidation = false;
    private bool _usingRanged;
    private bool _busyAttacking;

    private float _currentChargePercentage;

    [SerializeField] private CombatMoveSet _defaultMeleeMoveSet;
    [ReadOnly][SerializeField] private CombatMoveSet _currentMeleeMoveSet;
    [SerializeField] private CombatMoveSet _defaultRangeMoveSet;
    [ReadOnly][SerializeField] private CombatMoveSet _currentRangeMoveSet;
    [ReadOnly][SerializeField] private UseableAbility _dynamicUseable, _staticUseable;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponentsRefrences = playerComponents;
        _glide = _playerComponentsRefrences.GetPlayerGlide;
        _playerAcidation = _playerComponentsRefrences.GetPlayerAcidation;


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

    public PlayerComponentsRefrences GetPlayerRefs => _playerComponentsRefrences;
    public CombatMoveSet GetDefaultMoveSet => _defaultMeleeMoveSet;
    public CombatMoveSet GetCurrentMeleeMoveSet => _currentMeleeMoveSet;
    public CombatMoveSet GetCurrentRangeMoveSet => _currentRangeMoveSet;
    public UseableAbility GetCurrentStaticUseable => _staticUseable;
    public UseableAbility GetCurrentDynamicUseable => _dynamicUseable;
    public bool GetIsBusyAttacking => _busyAttacking;
    public bool GetCanAttack => _canAttack;
    public bool GetAcidation => _acidation;
    private bool CanPlayerAttack => !_glide.IsGliding() && _canAttack;
    public float GetChargePercentage => _currentChargePercentage;

    private void PlayerUpdate()
    {
        PerformMoveSetAction(moveset => moveset.MoveSetUpdate());
    }

    public void OnLightAttack()
    {
        PerformMoveSetAction(moveset => moveset.OnLightAttack());
    }

    public void OnReleaseLightAttack()
    {
        PerformMoveSetAction(moveset => moveset.OnReleaseLightAttack());
    }

    public void OnHeavyAttack()
    {
        PerformMoveSetAction(moveset => moveset.OnHeavyAttack());
    }

    public void OnReleaseHeavyAttack()
    {
        PerformMoveSetAction(moveset => moveset.OnReleaseHeavyAttack());
    }

    public void ClearAttacks()
    {
        PerformMoveSetAction(moveset => moveset.ResetAttacks());
    }

    private void PerformMoveSetAction(Action<CombatMoveSet> moveSetAction)
    {
        if (CanPlayerAttack)
        {
            CombatMoveSet currentMoveSet = _usingRanged ? _currentRangeMoveSet : _currentMeleeMoveSet;
            moveSetAction?.Invoke(currentMoveSet);
        }
    }

    public void OnStaticUseable()
    {
        PerformUseableAction(a => a.OnPress(),_staticUseable);
    }
    public void OnStaticUseableRelease()
    {
        PerformUseableAction(a => a.OnRelease(), _staticUseable);
    }
    public void OnDynamicUseable()
    {
        PerformUseableAction(a => a.OnPress(), _dynamicUseable);
    }
    public void OnDynamicUseableRelease()
    {
        PerformUseableAction(a => a.OnRelease(), _dynamicUseable);
    }

    private void PerformUseableAction(Action<UseableAbility> action, UseableAbility useable)
    {
        if (CanPlayerAttack && useable != null)
        {
            action?.Invoke(useable);
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

    public void SetMeleeMoveSet(CombatMoveSet meleeMoveSet)
    {
        SetMoveSet(ref _currentMeleeMoveSet, meleeMoveSet, _defaultMeleeMoveSet);
    }

    public void SetRangeMoveSet(CombatMoveSet rangeMoveSet)
    {
        SetMoveSet(ref _currentRangeMoveSet, rangeMoveSet, _defaultRangeMoveSet);
    }

    private void SetMoveSet(ref CombatMoveSet currentMoveSet, CombatMoveSet newMoveSet, CombatMoveSet defaultMoveSet)
    {
        ClearAttacks();
        SetOrDestroy(ref currentMoveSet, newMoveSet, defaultMoveSet);
        currentMoveSet.MoveSetStart(this);
        ClearAttacks();
        HandleSubscribtion();
    }

    public void SetStaticUseable(UseableAbility useable)
    {
        SetUseable(ref _staticUseable, useable);
    }

    public void SetDynamicUseable(UseableAbility useable)
    {
        SetUseable(ref _dynamicUseable, useable);
    }

    private void SetUseable(ref UseableAbility currentUseable, UseableAbility newUseable)
    {
        SetOrDestroy(ref currentUseable, newUseable);
        if (currentUseable != null)
        {
            currentUseable.UseableStart(this);
        }
    }

    private void SetOrDestroy<T>(ref T current, T newInstance, T defaultInstance = null) where T : Component
    {
        if (current != defaultInstance)
        {
            Destroy(current.gameObject);
        }

        if (newInstance == null)
        {
            newInstance = defaultInstance;
        }
        else
        {
            string name = newInstance.gameObject.name;
            newInstance = Instantiate(newInstance, transform.position, Quaternion.identity, transform);
            newInstance.gameObject.name = name;
        }

        current = newInstance;
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
