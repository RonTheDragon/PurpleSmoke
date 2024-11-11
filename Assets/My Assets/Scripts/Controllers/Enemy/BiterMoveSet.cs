using UnityEngine;

public class BiterMoveSet : EnemyCombatMoveSet
{
    [SerializeField] private LightAttack TestingAttack;
    [SerializeField] private AttackTest TestingAttack2;
    private EnemyAnimations _enemyAnimations;
    [SerializeField] private Damage _damager;
    private EnemyComponentRefrences _enemyComponentRefrences;

    public override void MoveSetStart(CombatSystem combatSystem)
    {
        base.MoveSetStart(combatSystem);
        _enemyAttacks.Add(TestingAttack);
        _enemyAttacks.Add(TestingAttack2);
        _enemyComponentRefrences = _enemyCombatSystem.GetEnemyComponentRefrences;
        _enemyAnimations = _enemyComponentRefrences.GetEnemyAnimations;
        _damager.SetOwner(_enemyComponentRefrences.GetCombatRules);
    }

    protected override void PerformAttack(EnemyAttackData attack)
    {
        base.PerformAttack(attack);
        if (attack is AttackTest) 
        {
            AttackTest a = (AttackTest)attack;
            Debug.Log(a.text);
        }
        if (attack is LightAttack)
        {
            LightAttack a = (LightAttack)attack;
            _enemyAnimations.PlayAnimation(a.text);
            _damager.SetDamage(a.Damage);
            _damager.SetKnock(a.Knockback, Random.Range(a.Knockout.x,a.Knockout.y));
        }
    }

    [System.Serializable]
    public class AttackTest : EnemyAttackData
    {
        public string text;
    }

    [System.Serializable]
    class LightAttack : AttackTest
    {
        public float Damage;
        public Vector2 Knockback, Knockout;
    }
}
