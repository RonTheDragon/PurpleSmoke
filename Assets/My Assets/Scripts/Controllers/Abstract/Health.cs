using System;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] protected ParticleSystem _acidSmoke;
    protected float _removeAcidAfterTimeLeft;

    protected CharacterKnockback _knockback;
    protected CharacterKnockout _knockout;
    private Vector2 _storedKnockBack;
    private float _storedKnockout;
    private Vector3 _knockedFrom;
    private float _currentKnockDelay;
    private float _highestKnockback;
    [SerializeField] private float _knockDelay = 0.1f;
    protected CombatRules _lastAttacker;

    protected bool _isDead = false;

    public Action OnDeath;
    public virtual void TakeDamage(float damageAmount, CombatRules Attacker)
    {
        _lastAttacker = Attacker;
        _currentHealth -= CalculateDamage(damageAmount);
        CheckIfDied();
    }
    public virtual void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation)
    {
        if (_isDead) return;
        if (_currentKnockDelay == 0)
        {
            _currentKnockDelay = _knockDelay;
        }
        _storedKnockBack += knockback;
        _storedKnockout += knockout;
        if (_highestKnockback < knockback.x)
        {
            _highestKnockback = knockback.x;
            _knockedFrom = attackLocation;
        }
    }
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
        HandleAcid();
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

            ParticleSystem.EmissionModule emission = _acidSmoke.emission;
            emission.rateOverTime = _currentAcidity/5;

            AcidPoison();

            AcidRemoval();
        }
    }

    protected void HandleKnock()
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
            _knockback.TakeKnockback(_storedKnockBack, _knockedFrom);
            _knockout.RecieveKnockout(_storedKnockout);

            _storedKnockBack = Vector2.zero;
            _storedKnockout = 0;
            _highestKnockback = 0;
        }
    }

    protected virtual void AcidPoison()
    {
        _currentHealth -= Mathf.Lerp(0, _acidDOT, _currentAcidity / _maxAcidity) * Time.deltaTime;
        CheckIfDied(); 
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

    public virtual void Die()
    {
        _currentHealth = 0;
        _isDead = true;
        OnDeath?.Invoke();
        OnDeath = null;
        _lastAttacker.KilledEnemy();
    }

    public void TakeFireDamage(float fire)
    {
       // throw new NotImplementedException();
    }

    public bool GetIsDead => _isDead;

    public CombatRules GetLastAttacker => _lastAttacker;
}
