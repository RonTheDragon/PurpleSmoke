
using UnityEngine;

public class EnemyKnockout : CharacterKnockout, IEnemyComponent
{
    private EnemyHealth _enemyHealth;
    private EnemyWalk _enemyWalk;
    private EnemyAnimations _enemyAnimations;
    private EnemyCombatSystem _enemyCombatSystem;
    private EnemyAttackMovement _enemyAttackMovement;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _enemyHealth = enemyComponents.GetEnemyHealth;
        _enemyWalk = enemyComponents.GetEnemyWalk;
        _enemyAnimations = enemyComponents.GetEnemyAnimations;
        _enemyCombatSystem = enemyComponents.GetEnemyCombatSystem;
        _enemyAttackMovement = enemyComponents.GetEnemyAttackMovement;
        enemyComponents.OnUpdate += EnemyUpdate;
        OnCanGetUp += (b) => { if (b) TryToGetUp(); };
    }

    private void EnemyUpdate()
    {
        if (_enemyHealth.GetIsDead) return;
        StunCheckLoop();
    }

    protected override void ClearAttacks()
    {
        
    }

    protected override void PlayAnimation(string animationName)
    {
        if (_enemyHealth.GetIsDead) return;
        _enemyAnimations.PlayAnimation(animationName);
    }

    protected override void SetAnimationWeight(float weight)
    {
        _enemyAnimations.SetLayerWeight(1, weight);
    }

    public override void StunCharacter()
    {
        base.StunCharacter();
        _enemyWalk.AddNotMovingReason("Stun");
        _enemyWalk.AddNotNavmeshReason("Stun");
        _enemyAttackMovement.StopMovement();
        _enemyCombatSystem.SetCanAttack(false);
    }

    public override void UnStunCharacter()
    {
        base.UnStunCharacter();
        _enemyWalk.RemoveNotMovingReason("Stun");
        _enemyWalk.RemoveNotNavmeshReason("Stun");
        _enemyCombatSystem.SetCanAttack(true);
    }
}
