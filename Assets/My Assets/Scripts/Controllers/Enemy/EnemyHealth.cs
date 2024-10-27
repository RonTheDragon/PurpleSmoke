using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health , IEnemyComponent
{
    private EnemyDeath _enemyDeath;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _enemyDeath = EnemyComponents.GetEnemyDeath;
        HealToMax();
    }

    public override void Die()
    {
        _enemyDeath.Die();
    }

    public override void TakeKnock(Vector2 knockback, float knockout, Vector3 attackLocation)
    {
        
    }
}
