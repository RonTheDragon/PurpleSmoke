
public class EnemyKnockout : CharacterKnockout, IEnemyComponent
{
    private EnemyHealth _enemyHealth;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _enemyHealth = enemyComponents.GetEnemyHealth;
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
}
