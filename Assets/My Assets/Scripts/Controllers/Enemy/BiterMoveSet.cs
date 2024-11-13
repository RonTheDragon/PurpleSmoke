using UnityEngine;

public class BiterMoveSet : EnemyCombatMoveSet
{
    [SerializeField] private LightAttack Bite;
    [SerializeField] private LightAttackMovement Jump;
    private EnemyAnimations _enemyAnimations;
    [SerializeField] private Damage _damager;
    private EnemyComponentRefrences _enemyComponentRefrences;
    private EnemyAttackMovement _enemyAttackMovement;
    private EnemyChase _enemyChase;

    public override void MoveSetStart(CombatSystem combatSystem)
    {
        base.MoveSetStart(combatSystem);
        _enemyAttacks.Add(Bite);
        _enemyAttacks.Add(Jump);
        _enemyComponentRefrences = _enemyCombatSystem.GetEnemyComponentRefrences;
        _enemyAnimations = _enemyComponentRefrences.GetEnemyAnimations;
        _enemyAttackMovement = _enemyComponentRefrences.GetEnemyAttackMovement;
        _enemyChase = _enemyComponentRefrences.GetEnemyChase;
        _damager.SetOwner(_enemyComponentRefrences.GetCombatRules);
    }

    protected override void PerformAttack(EnemyAttackData attack)
    {
        base.PerformAttack(attack);
        _enemyAttackMovement.ClearTarget();
        if (attack is LightAttack)
        {
            LightAttack a = (LightAttack)attack;
            _enemyAnimations.PlayAnimation(a.Animation);
            _damager.SetDamage(a.Damage);
            _damager.SetKnock(a.Knockback, Random.Range(a.Knockout.x,a.Knockout.y));
            if (attack is LightAttackMovement)
            {
                LightAttackMovement aa = (LightAttackMovement)attack;
                _enemyAttackMovement.SetTarget(_enemyChase.GetTarget);
                _enemyAttackMovement.SetMovement(aa.Movement);
            }
        }
    }


    [System.Serializable]
    class LightAttack : EnemyAttackData
    {
        public string Animation;
        public float Damage;
        public Vector2 Knockback, Knockout;
    }

    [System.Serializable]
    class LightAttackMovement : LightAttack
    {
        public Vector3 Movement;
    }
}
