using System;
using UnityEngine;

public class PlayerHealth : Health , IPlayerComponent
{
    public Action<float> OnPlayerHealthChange;
    
    private PlayerDeath _playerDeath;
    private PlayerAcidation _playerAcidation;
    private PlayerAimMode _playerAimMode;
    private PlayerInputsHandler _playerInputsHandler;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _knockback = playerComponents.GetPlayerKnockback;
        _knockout = playerComponents.GetPlayerKnockout;
        _playerDeath = playerComponents.GetPlayerDeath;
        _playerAcidation = playerComponents.GetPlayerAcidation;
        _playerAimMode = playerComponents.GetPlayerAimMode;
        _playerInputsHandler = playerComponents.GetPlayerInputsHandler;
        HealToMax();
        UpdateHealthUI();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        HandleKnock();
        HandleAcid();
    }

    public override void TakeDamage(float damageAmount, CombatRules Attacker)
    {
        if (GetIsDead) return;
        base.TakeDamage(damageAmount, Attacker);
        UpdateHealthUI();
    }

    

    

    [ContextMenu("Die")]
    public override void Die()
    {
        base.Die();
        UpdateHealthUI();
        _playerAcidation.SetAcidationToEmpty();
        _playerAcidation.SetCanGenerateAcidation(false);
        _playerDeath.Die();
        _playerAimMode.SetLockHeadAim(true);
        _playerInputsHandler.CloseInventoryAbruptly();
    }

    [ContextMenu("Heal To Max")]
    public override void HealToMax()
    {
        base.HealToMax();
        UpdateHealthUI();
        _playerAcidation.SetAcidationToMax();
        _playerAcidation.SetCanGenerateAcidation(true);
        _playerAimMode.SetLockHeadAim(false);
    }

    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        OnPlayerHealthChange?.Invoke(_currentHealth / _maxHealth);
    }

    protected override void AcidPoison()
    {
        base.AcidPoison();
        UpdateHealthUI();
    }
}
