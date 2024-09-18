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

    [Header("Acid")]
    [SerializeField] private float _maxAcidity = 100;
    [SerializeField] private float _currentAcidity = 0;
    [SerializeField] private float _acidExtraDamagePercentage = 50;
    [SerializeField] private float _acidDOT = 5;
    [SerializeField] private float _acidRemovalPerSec = 30;
    [SerializeField] private float _removeAcidAfter = 1;
    private float _removeAcidAfterTimeLeft;

    private Vector2 _storedKnockBack;
    private float _storedKnockout;
    private Vector3 _knockedFrom;
    [SerializeField] private float _knockDelay = 0.1f;
    private float _currentKnockDelay;

    private float _highestKnockback;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerKnockback = playerComponents.GetPlayerKnockback;
        _playerKnockout = playerComponents.GetPlayerKnockout;
        _playerDeath = playerComponents.GetPlayerDeath;
        _playerAcidation = playerComponents.GetPlayerAcidation;
        _playerAimMode = playerComponents.GetPlayerAimMode;
        HealToMax();
        UpdateHealthUI();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        HandleKnock();
        HandleAcid();
    }

    public override void TakeDamage(float damageAmount, GameObject Attacker)
    {
        if (_isDead) return;

        _currentHealth -= CalculateDamage(damageAmount);
        if (CheckIfDied()) return;

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


    private bool CheckIfDied()
    {
        if (_currentHealth <= 0 )
        {
            Die();
            return true;
        }
        return false;
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
    }

    [ContextMenu("Heal To Max")]
    public override void HealToMax()
    {
        base.HealToMax();
        UpdateHealthUI();
        _currentAcidity = 0;
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

    private float CalculateDamage(float damage)
    {
        if (_currentAcidity>0) 
        {
            damage += damage* Mathf.Lerp(0, _acidExtraDamagePercentage/100, _currentAcidity/_maxAcidity);
        }
        return damage;
    }

    public override void TakeAcidDamage(float acid)
    {
        if (acid > 0)
        {
            _currentAcidity += acid;
            _removeAcidAfterTimeLeft = _removeAcidAfter;
        }
    }

    private void HandleAcid()
    {
        if (_currentAcidity > 0 && !_isDead)
        {
            if (_currentAcidity > _maxAcidity)
            {
                _currentAcidity = _maxAcidity;
            }

            AcidPoison();

            AcidRemoval();
        }
    }

    private void AcidPoison()
    {
        _currentHealth -= Mathf.Lerp(0, _acidDOT, _currentAcidity / _maxAcidity) * Time.deltaTime;
        if (CheckIfDied()) return;
        UpdateHealthUI();
    }

    private void AcidRemoval()
    {
        if (_removeAcidAfterTimeLeft > 0)
        {
            _removeAcidAfterTimeLeft -= Time.deltaTime;
        }
        else
        {
            _currentAcidity -= _acidRemovalPerSec * Time.deltaTime;
        }
    }
}
