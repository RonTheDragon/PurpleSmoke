using UnityEngine;

public class AcidShotgunProjectile : Projectile
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleDamage _particleDamage;

    public override void SetProjectile(CombatRules owner, ProjectileAttack projectileAttack)
    {
        if (projectileAttack is AcidShotgunAttack)
        {
            AcidShotgunAttack attack = (AcidShotgunAttack)projectileAttack;
            ParticleSystem.MainModule main =_particleSystem.main;
            main.startLifetime = attack.TimeBeforeDissolve;
            main.startSpeed = attack.ProjectileSpeed;
            ParticleSystem.ShapeModule shape = _particleSystem.shape;
            shape.angle = attack.Spread;
            _particleSystem.Emit(attack.PelletAmount);
            _particleDamage.SetOwner(owner);
            _particleDamage.SetDamage(attack.ProjectileDamage);
            _particleDamage.SetKnock(attack.Knockback,attack.Knockout);
            _particleDamage.SetAcidDamage(attack.ProjectileAcidDamage);
        }
    }
}
