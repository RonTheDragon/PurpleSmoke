using UnityEngine;

public class EnemyHealth : Health , IEnemyComponent
{
    private EnemyDeath _enemyDeath;
    private EnemyKnockback _enemyKnockback;
    private EnemyKnockout _enemyKnockout;
    private EnemyChase _enemyChase;
    private EnemyDetection _enemyDetection;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _enemyKnockback = EnemyComponents.GetEnemyKnockback;
        _enemyKnockout = EnemyComponents.GetEnemyKnockout;
        _enemyDeath = EnemyComponents.GetEnemyDeath;
        _enemyChase = EnemyComponents.GetEnemyChase;
        _enemyDetection = EnemyComponents.GetEnemyDetection;
        HealToMax();
    }

    public override void TakeDamage(float damageAmount, CombatRules Attacker)
    {
        _enemyChase.Agro();
        _enemyDetection.TargetAgro(Attacker.transform);
        base.TakeDamage(damageAmount, Attacker);
    }

    public override void Die()
    {
        _enemyDeath.Die();
    }

    public override void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation)
    {
        _enemyKnockback.TakeKnockback(knockback, attackLocation);
        _enemyKnockout.RecieveKnockout(knockout);
    }
}
