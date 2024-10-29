using UnityEngine;

public class EnemyKnockback : CharacterKnockback, IEnemyComponent
{
    private EnemyWalk _enemyWalk;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _characterController = enemyComponents.GetCharacterController;
        _enemyWalk = enemyComponents.GetEnemyWalk;
        enemyComponents.OnUpdate += EnemyUpdate;
    }

    public override void TakeKnockback(Vector2 knockback, Vector3 attackLocation)
    {
        if (knockback.magnitude > 0.1f)
        {
            _enemyWalk.AddNotNavmeshReason("Knockback");
        }
        base.TakeKnockback(knockback, attackLocation);
    }

    private void EnemyUpdate()
    {
        KnockbackLoop();
    }

    protected override void EndKnockback()
    {
        base.EndKnockback();
        _enemyWalk.RemoveNotNavmeshReason("Knockback");
    }
}
