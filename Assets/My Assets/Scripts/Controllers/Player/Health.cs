using UnityEngine;

public abstract class Health : MonoBehaviour , IDamageable
{
    [SerializeField] protected float _maxHealth;
    [ReadOnly][SerializeField] protected float _currentHealth;
    protected bool _isDead = false;
    public abstract void TakeDamage(float damageAmount, Vector2 knockback, float knockout , Vector3 attackLocation, GameObject Attacker);

    [ContextMenu("Heal To Max")]
    public void HealToMax()
    {
        _currentHealth = _maxHealth;
        _isDead=false;
    }

    public abstract void Die();
}
