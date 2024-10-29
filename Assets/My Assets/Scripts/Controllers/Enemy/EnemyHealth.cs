using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health , IEnemyComponent
{
    private EnemyDeath _enemyDeath;
    private EnemyKnockback _enemyKnockback;
    private EnemyKnockout _enemyKnockout;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _enemyKnockback = EnemyComponents.GetEnemyKnockback;
        _enemyKnockout = EnemyComponents.GetEnemyKnockout;
        _enemyDeath = EnemyComponents.GetEnemyDeath;
        HealToMax();
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
