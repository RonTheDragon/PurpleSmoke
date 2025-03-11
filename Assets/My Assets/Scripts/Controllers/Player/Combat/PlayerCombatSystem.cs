using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : CombatSystem, IPlayerComponent
{
    //public Action<float> OnChargeChange;

    private PlayerComponentsRefrences _playerComponentsRefrences;
    private PlayerGlide _glide;
    private PlayerAcidation _playerAcidation;
    private PlayerInventory _playerInventory;
    private bool _acidation = false;
    private bool _usingRanged;
    private bool _busyAttacking;
    [SerializeField] private Transform _itemsLocation, _effectsList, _rightHand, _leftHand;
    [SerializeField] private List<Damage> _damagers;

    InventoryItemUI _currentMeleeItemUI, _currentRangeItemUI, _currentDynamicItemUI, _currentStaticItemUI;


    //private float _currentChargePercentage;

    [SerializeField] private PlayerCombatMoveSet _defaultMeleeMoveSet;
    [ReadOnly][SerializeField] private PlayerCombatMoveSet _currentMeleeMoveSet;
    [SerializeField] private PlayerCombatMoveSet _defaultRangeMoveSet;
    [ReadOnly][SerializeField] private PlayerCombatMoveSet _currentRangeMoveSet;
    [ReadOnly][SerializeField] private UseableAbility _dynamicUseable, _staticUseable;
    [ReadOnly][SerializeField] private List<Consumable> _consumedConsumables;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponentsRefrences = playerComponents;
        _glide = _playerComponentsRefrences.GetPlayerGlide;
        _playerAcidation = _playerComponentsRefrences.GetPlayerAcidation;
        _playerInventory = _playerComponentsRefrences.GetPlayerInventory;


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

        HandleEquipment();
    }

    public PlayerComponentsRefrences GetPlayerRefs => _playerComponentsRefrences;
    public PlayerCombatMoveSet GetDefaultMoveSet => _defaultMeleeMoveSet;
    public PlayerCombatMoveSet GetCurrentMeleeMoveSet => _currentMeleeMoveSet;
    public PlayerCombatMoveSet GetCurrentRangeMoveSet => _currentRangeMoveSet;
    public UseableAbility GetCurrentStaticUseable => _staticUseable;
    public UseableAbility GetCurrentDynamicUseable => _dynamicUseable;
    public bool GetIsBusyAttacking => _busyAttacking;
    public bool GetAcidation => _acidation;
    private bool CanPlayerAttack => !_glide.IsGliding() && _canAttack;
    //public float GetChargePercentage => _currentChargePercentage;
    public List<Damage> GetDamagers => _damagers;
    public Transform GetRightHand => _rightHand;
    public Transform GetLeftHand => _leftHand;

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

    protected void PerformMoveSetAction(Action<PlayerCombatMoveSet> moveSetAction)
    {
        if (CanPlayerAttack)
        {
            PlayerCombatMoveSet currentMoveSet = _usingRanged ? _currentRangeMoveSet : _currentMeleeMoveSet;
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

    public void SetAcidation(bool acidation)
    {
        _acidation = acidation;
    }
    public void SetBusyAttacking(bool busyAttacking)
    {
        _busyAttacking = busyAttacking;
    }
    //public void SetChargePercentage(float charge)
    //{
    //    _currentChargePercentage = charge;
    //    OnChargeChange?.Invoke(charge);
    //}

    public void SetUsingRanged(bool usingRanged)
    {
        ClearAttacks();
        _usingRanged = usingRanged;
        ClearAttacks();
        HandleEquipment();
    }

    public void SetMeleeMoveSet(PlayerCombatMoveSet meleeMoveSet, InventoryItemUI item)
    {
        SetMoveSet(ref _currentMeleeMoveSet, meleeMoveSet, _defaultMeleeMoveSet);
        _currentMeleeItemUI = item;
    }

    public void SetRangeMoveSet(PlayerCombatMoveSet rangeMoveSet, InventoryItemUI item)
    {
        SetMoveSet(ref _currentRangeMoveSet, rangeMoveSet, _defaultRangeMoveSet);
        _currentRangeItemUI = item;
    }

    private void SetMoveSet(ref PlayerCombatMoveSet currentMoveSet, PlayerCombatMoveSet newMoveSet, PlayerCombatMoveSet defaultMoveSet)
    {
        ClearAttacks();
        currentMoveSet.SetEquippedState(false);
        SetOrDestroy(ref currentMoveSet, newMoveSet, defaultMoveSet);
        currentMoveSet.MoveSetStart(this);
        ClearAttacks();
        HandleEquipment();
    }

    public void SetStaticUseable(UseableAbility useable, InventoryItemUI item)
    {
        SetUseable(ref _staticUseable, useable);
        _currentStaticItemUI = item;
    }

    public void SetDynamicUseable(UseableAbility useable, InventoryItemUI item)
    {
        SetUseable(ref _dynamicUseable, useable);
        _currentDynamicItemUI = item;
    }

    private void SetUseable(ref UseableAbility currentUseable, UseableAbility newUseable)
    {
        SetOrDestroy(ref currentUseable, newUseable);
        if (currentUseable != null)
        {
            currentUseable.UseableStart(this);
        }
    }

    public bool ConsumeConsumable(Consumable consumable)
    {
        bool consumed = consumable.CheckIfCanConsume(this);
        if (!consumed) 
            return false;
        
        foreach (Consumable c in _consumedConsumables)
        {
            if (c.GetEffectName == consumable.GetEffectName)
            {
                c.FinishConsumable();
                _consumedConsumables.Remove(c);
                Destroy(c.gameObject); 
                break;
            }
        }
        consumable = Instantiate(consumable, transform.position, Quaternion.identity, _effectsList);
        consumable.Consume(this);
        _consumedConsumables.Add(consumable);
        return true;
    }

    public void ConsumeMelee()
    {
        
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
            newInstance = Instantiate(newInstance, transform.position, Quaternion.identity, _itemsLocation);
            newInstance.gameObject.name = name;
        }

        current = newInstance;
    }


    private void HandleEquipment()
    {
        if (_usingRanged)
        {
            _currentRangeMoveSet.SetEquippedState(true);
            _currentMeleeMoveSet.SetEquippedState(false);
        }
        else
        {
            _currentRangeMoveSet.SetEquippedState(false);
            _currentMeleeMoveSet.SetEquippedState(true);
        }
    }

    public void SpendMelee(int amount = 1)
    {
        _playerInventory.RemoveAmountFromItem(_currentMeleeItemUI,amount);
    }

    public bool IsMeleeEmpty()
    {
       return _playerInventory.IsItemEmpty(_currentMeleeItemUI);
    }

    public void DeleteIfEmptyMelee()
    {
        if (_currentMeleeItemUI==null) return;
        if(IsMeleeEmpty())
        {
            _playerInventory.RemoveWholeItem(_currentMeleeItemUI);
            SetMeleeMoveSet(null, null);
            _playerInventory.MeleeSlotClear();
            
        }
    }
}
