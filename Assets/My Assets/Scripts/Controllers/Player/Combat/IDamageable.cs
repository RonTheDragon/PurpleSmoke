using UnityEngine;

public interface IDamageable
{
    public abstract void TakeDamage(float damageAmount, CombatRules Attacker);
    public abstract void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation);
    public abstract void TakeAcidDamage(float acid);
    public abstract void TakeFireDamage(float fire);
}
