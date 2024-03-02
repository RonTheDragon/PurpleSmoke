using UnityEngine;

public class PlayerHealth : Health , IPlayerComponent
{
    private PlayerKnockback _playerKnockback;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerKnockback = playerComponents.GetPlayerKnockback();
    }

    public override void TakeDamage(float damageAmount, Vector2 knockback, Vector3 attackLocation, GameObject Attacker)
    {
        _currentHealth -= damageAmount;
        if (CheckIfDied()) return;

        _playerKnockback.TakeKnockback(knockback, attackLocation);
    }

    private bool CheckIfDied()
    {
        if (_currentHealth <= 0 && _isDead == false )
        {
            Die();
            return true;
        }
        return false;
    }

    protected override void Die()
    {
        _currentHealth = 0;
        _isDead = true;
    }
}
