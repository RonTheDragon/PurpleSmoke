using System;
using UnityEngine;

public class PlayerHealth : Health , IPlayerComponent
{
    public Action<float> OnPlayerHealthChange;
    private PlayerKnockback _playerKnockback;
    private PlayerKnockout _playerKnockout;
    private PlayerDeath _playerDeath;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerKnockback = playerComponents.GetPlayerKnockback();
        _playerKnockout = playerComponents.GetPlayerKnockout();
        _playerDeath = playerComponents.GetPlayerDeath();
        HealToMax();
        UpdateHealthUI();
    }

    public override void TakeDamage(float damageAmount, Vector2 knockback, float knockout , Vector3 attackLocation, GameObject Attacker)
    {
        if (_isDead) return;

        _currentHealth -= damageAmount;
        if (CheckIfDied()) return;

        _playerKnockback.TakeKnockback(knockback, attackLocation);
        _playerKnockout.RecieveKnockout(knockout);
        UpdateHealthUI();
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
        _playerDeath.Die();
    }

    private void UpdateHealthUI()
    {
        OnPlayerHealthChange?.Invoke(_currentHealth / _maxHealth);
    }
}
