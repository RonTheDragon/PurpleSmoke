using System;
using UnityEngine;

public class PlayerHealth : Health , IPlayerComponent
{
    public Action<float> OnPlayerHealthChange;
    private PlayerKnockback _playerKnockback;
    private PlayerKnockout _playerKnockout;
    private PlayerDeath _playerDeath;
    private PlayerAcidation _playerAcidation;
    private PlayerAimMode _playerAimMode;
    private PlayerInputsHandler _playerInputsHandler;
    private CharacterController _characterController;

    private Vector2 _storedKnockBack;
    private float _storedKnockout;
    private Vector3 _knockedFrom;
    [SerializeField] private float _knockDelay = 0.1f;
    private float _currentKnockDelay;

    private float _highestKnockback;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerKnockback = playerComponents.GetPlayerKnockback;
        _playerKnockout = playerComponents.GetPlayerKnockout;
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
        base.TakeDamage(damageAmount, Attacker);
        UpdateHealthUI();
    }

    public override void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation)
    {
        if (_isDead) return;
        if (_currentKnockDelay == 0)
        {
            _currentKnockDelay = _knockDelay;
        }
        _storedKnockBack += knockback;
        _storedKnockout += knockout;
        if (_highestKnockback< knockback.x)
        {
            _highestKnockback = knockback.x;
            _knockedFrom = attackLocation;
        }
    }

    private void HandleKnock()
    {
        if (_isDead)
        {
            _currentKnockDelay = 0;
            _storedKnockBack = Vector2.zero;
            _storedKnockout = 0;
            _highestKnockback = 0;
            return;
        }

        if (_currentKnockDelay > 0) { _currentKnockDelay -= Time.deltaTime; }
        else if (_currentKnockDelay < 0)
        {
            _currentKnockDelay = 0;
            _playerKnockback.TakeKnockback(_storedKnockBack, _knockedFrom);
            _playerKnockout.RecieveKnockout(_storedKnockout);

            _storedKnockBack = Vector2.zero;
            _storedKnockout = 0;
            _highestKnockback = 0;
        }
    }

    [ContextMenu("Die")]
    public override void Die()
    {
        _currentHealth = 0;
        _isDead = true;
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
