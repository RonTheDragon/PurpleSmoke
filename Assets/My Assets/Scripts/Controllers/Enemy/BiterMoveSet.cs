using UnityEngine;

public class BiterMoveSet : EnemyCombatMoveSet
{
    [SerializeField] private AttackTest TestingAttack;
    [SerializeField] private AttackTest TestingAttack2;
    public override void MoveSetStart(CombatSystem combatSystem)
    {
        base.MoveSetStart(combatSystem);
        _enemyAttacks.Add(TestingAttack);
        _enemyAttacks.Add(TestingAttack2);
    }

    protected override void PerformAttack(EnemyAttackData attack)
    {
        base.PerformAttack(attack);
        if (attack is AttackTest) 
        {
            AttackTest a = (AttackTest)attack;
            Debug.Log(a.text);
        }
    }

    [System.Serializable]
    public class AttackTest : EnemyAttackData
    {
        public string text;
    }
}
