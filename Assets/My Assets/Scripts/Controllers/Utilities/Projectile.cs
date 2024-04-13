using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public abstract void SetProjectile(GameObject owner, ProjectileAttack projectileAttack);
}
