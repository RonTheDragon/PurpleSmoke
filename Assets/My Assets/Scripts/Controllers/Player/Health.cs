using UnityEngine;

public abstract class Health : MonoBehaviour , IDamageable
{
    [SerializeField] protected float _maxHealth;
    [ReadOnly][SerializeField] protected float _currentHealth;
    protected bool _isDead = false;
    public abstract void TakeDamage(float damageAmount, Vector2 knockback, Vector3 attackLocation, GameObject Attacker);

    [ContextMenu("Heal To Max")]
    protected void HealToMax()
    {
        _currentHealth = _maxHealth;
    }

    [ContextMenu("Die")]
    protected abstract void Die();
}
