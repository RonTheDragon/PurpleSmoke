using UnityEngine;

public interface IDamageable
{
    public abstract void TakeDamage(float damageAmount, GameObject Attacker);
    public abstract void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation);
    public abstract void TakeAcidDamage(float acid);
}
