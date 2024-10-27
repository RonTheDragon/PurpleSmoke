using UnityEngine;

public abstract class Health : MonoBehaviour , IDamageable
{
    [SerializeField] protected float _maxHealth;
    [ReadOnly][SerializeField] protected float _currentHealth;

    [Header("Acid")]
    [SerializeField] protected float _maxAcidity = 100;
    [ReadOnly][SerializeField] protected float _currentAcidity = 0;
    [SerializeField] protected float _acidExtraDamagePercentage = 50;
    [SerializeField] protected float _acidDOT = 5;
    [SerializeField] protected float _acidRemovalPerSec = 30;
    [SerializeField] protected float _removeAcidAfter = 1;
    protected float _removeAcidAfterTimeLeft;

    protected bool _isDead = false;
    public virtual void TakeDamage(float damageAmount, GameObject Attacker)
    {
        if (_isDead) return;

        _currentHealth -= CalculateDamage(damageAmount);
        if (CheckIfDied()) return;
    }
    public abstract void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation);
    public virtual void TakeAcidDamage(float acid)
    {
        if (acid > 0)
        {
            _currentAcidity += acid;
            _removeAcidAfterTimeLeft = _removeAcidAfter;
        }
    }

    public bool IsHealthFull => _currentHealth >= _maxHealth;
    public float MaxHP => _maxHealth;
    public virtual void HealToMax()
    {
        _currentHealth = _maxHealth;
        _isDead=false;
        _currentAcidity = 0;
    }

    protected float CalculateDamage(float damage)
    {
        if (_currentAcidity > 0)
        {
            damage += damage * Mathf.Lerp(0, _acidExtraDamagePercentage / 100, _currentAcidity / _maxAcidity);
        }
        return damage;
    }

    public virtual void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
    }

    protected bool CheckIfDied()
    {
        if (_currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected void HandleAcid()
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

    protected virtual void AcidPoison()
    {
        _currentHealth -= Mathf.Lerp(0, _acidDOT, _currentAcidity / _maxAcidity) * Time.deltaTime;
        if (CheckIfDied()) return;  
    }

    protected void AcidRemoval()
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

    public abstract void Die();

    public bool GetIsDead => _isDead;
}
