using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damageAmount, Vector2 knockback, Vector3 attackLocation, GameObject Attacker);
}
