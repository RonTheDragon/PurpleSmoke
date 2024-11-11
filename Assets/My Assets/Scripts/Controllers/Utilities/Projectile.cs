using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public abstract void SetProjectile(CombatRules owner, ProjectileAttack projectileAttack);
}
