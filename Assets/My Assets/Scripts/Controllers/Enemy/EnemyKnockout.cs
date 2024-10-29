
public class EnemyKnockout : CharacterKnockout, IEnemyComponent
{
    private EnemyHealth _enemyHealth;
    private EnemyWalk _enemyWalk;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _enemyHealth = enemyComponents.GetEnemyHealth;
        _enemyWalk = enemyComponents.GetEnemyWalk;
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
        
    }

    protected override void SetAnimationWeight(float weight)
    {
        
    }

    public override void StunCharacter()
    {
        base.StunCharacter();
        _enemyWalk.SetCanMove(false);
    }

    public override void UnStunCharacter()
    {
        base.UnStunCharacter();
        _enemyWalk.SetCanMove(true);
    }
}
