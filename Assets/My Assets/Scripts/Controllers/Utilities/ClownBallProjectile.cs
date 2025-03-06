using UnityEngine;
using static ClownMoveSet;
using static CombatMoveSet;

public class ClownBallProjectile : MovingProjectile
{
    [SerializeField] private ProjectileFlameExplosionDamage damager;
    public override void SetProjectile(CombatRules owner, AttackData projectileAttack)
    {
        base.SetProjectile(owner, projectileAttack);
        if (projectileAttack is BallThrow)
        {
            BallThrow ball = (BallThrow)projectileAttack;
            _movement.SetVariablesAndLaunch(ball.Velocity, ball.Gravity);
            damager.SetOwner(owner);
            damager.SetDamage(ball.Damage);
            damager.SetAcidDamage(ball.AcidDamage);
            damager.SetFireDamage(ball.FireDamage);
            damager.SetKnock(ball.Knockback, Random.Range(ball.Knockout.x, ball.Knockout.y));
            damager.SetRadius(ball.Radius);
            damager.SetAttackData(ball);
        }
    }
}
