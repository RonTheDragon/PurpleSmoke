using UnityEngine;

public class AcidShootProjectile : MovingProjectile
{
    [SerializeField] private ProjectileDecalExplosionDamage damager;
    public override void SetProjectile(CombatRules owner, ProjectileAttack projectileAttack)
    {
        base.SetProjectile(owner, projectileAttack);
        if (projectileAttack is AcidSpitAttack)
        {
            AcidSpitAttack acidSpitAttack = (AcidSpitAttack)projectileAttack;
            _movement.SetVariablesAndLaunch(acidSpitAttack.ProjectileSpeed, acidSpitAttack.ProjectileGravity);
            damager.SetOwner(owner);
            damager.SetDamage(acidSpitAttack.ProjectileDamage);
            damager.SetAcidDamage(acidSpitAttack.ProjectileAcidDamage);
            damager.SetRadius(acidSpitAttack.ProjectileExplosionRange);
        }
    }
}
