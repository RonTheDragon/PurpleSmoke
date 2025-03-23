

public class EnemyHealth : Health , IEnemyComponent
{
    private EnemyDeath _enemyDeath;
    private EnemyChase _enemyChase;
    private EnemyDetection _enemyDetection;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _knockback = EnemyComponents.GetEnemyKnockback;
        _knockout = EnemyComponents.GetEnemyKnockout;
        _enemyDeath = EnemyComponents.GetEnemyDeath;
        _enemyChase = EnemyComponents.GetEnemyChase;
        _enemyDetection = EnemyComponents.GetEnemyDetection;
        EnemyComponents.OnUpdate += EnemyUpdate;
        HealToMax();
    }
    
    private void EnemyUpdate()
    {
        HandleAcid();
        HandleKnock();
    }

    public override void TakeDamage(float damageAmount, CombatRules Attacker)
    {
        if (GetIsDead) return;
        _enemyChase.Agro();
        _enemyDetection.TargetAgro(Attacker.transform);
        base.TakeDamage(damageAmount, Attacker);
    }

    public override void Die()
    {
        base.Die();
        _enemyDeath.Die();
       
    }

    public void Spawn()
    {
        if (_isDead)
        {
            HealToMax();
            _enemyDeath.Revive();
        }
    }
}
