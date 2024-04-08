using UnityEngine;

public class AcidShootProjectile : Projectile
{
    [SerializeField] private ProjectileDecalExplosionDamage damager;
    public override void SetProjectile(GameObject owner, ProjectileAttack projectileAttack)
    {
        base.SetProjectile(owner, projectileAttack);
        if (projectileAttack is AcidSpitAttack)
        {
            AcidSpitAttack acidSpitAttack = (AcidSpitAttack)projectileAttack;
            _movement.SetVariablesAndLaunch(acidSpitAttack.ProjectileSpeed, acidSpitAttack.ProjectileGravity);
            damager.SetOwner(owner);
            damager.SetDamage(acidSpitAttack.ProjectileDamage,Vector2.zero,0);
            damager.SetAcidDamage(acidSpitAttack.ProjectileAcidDamage);
            damager.SetRadius(acidSpitAttack.ProjectileExplosionRange);
        }
    }
}
