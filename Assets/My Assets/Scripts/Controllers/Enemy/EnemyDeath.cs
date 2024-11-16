using UnityEngine;

public class EnemyDeath : MonoBehaviour , IEnemyComponent
{
    //private EnemyWalk _enemyWalk;
    private EnemyKnockout _enemyKnockout;
    private EnemyAnimations _enemyAnimations;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        //_enemyWalk = enemyComponents.GetEnemyWalk;
        _enemyKnockout = enemyComponents.GetEnemyKnockout;
        _enemyAnimations = enemyComponents.GetEnemyAnimations;
    }

    public void Die()
    {
        //gameObject.SetActive(false);
        _enemyAnimations.PlayAnimation("Stumble");
        _enemyKnockout.StunCharacter();
    }
}
