using UnityEngine;
using static CombatMoveSet;

public class AcidShootProjectile : MovingProjectile
{
    [SerializeField] private ProjectileDecalExplosionDamage damager;
    public override void SetProjectile(CombatRules owner, AttackData projectileAttack)
    {
        base.SetProjectile(owner, projectileAttack);
        if (projectileAttack is AcidSpitAttack)
        {
            AcidSpitAttack acidSpitAttack = (AcidSpitAttack)projectileAttack;
            _movement.SetVariablesAndLaunch(acidSpitAttack.ProjectileSpeed, acidSpitAttack.ProjectileGravity);
            damager.SetOwner(owner);
            damager.SetDamage(acidSpitAttack.ProjectileDamage);
            damager.SetAcidDamage(acidSpitAttack.ProjectileAcidDamage);
            damager.SetKnock(acidSpitAttack.Knockback,Random.Range(acidSpitAttack.Knockout.x,acidSpitAttack.Knockout.y));
            damager.SetRadius(acidSpitAttack.ProjectileExplosionRange);
        }
    }
}
