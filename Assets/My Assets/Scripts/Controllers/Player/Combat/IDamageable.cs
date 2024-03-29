using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damageAmount, Vector2 knockback, float knockout, Vector3 attackLocation, GameObject Attacker);

    public abstract void TakeAcidDamage(float acid);
}
