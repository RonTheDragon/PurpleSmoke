using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCombatMoveSet : CombatMoveSet
{
    protected EnemyCombatSystem _enemyCombatSystem;
    protected List<EnemyAttackData> _enemyAttacks = new List<EnemyAttackData>();

    [SerializeField] protected float _tryAttackCooldown;
    protected float _tryAttackCD;

    public override void MoveSetStart(CombatSystem combatSystem)
    {
        _enemyCombatSystem = (EnemyCombatSystem)combatSystem;
    }

    public override void MoveSetUpdate()
    {
        AttackLoop();
    }

    protected virtual void AttackLoop()
    {
        if (_tryAttackCD > 0) { _tryAttackCD -= Time.deltaTime; return; }
        if (!_enemyCombatSystem.HasTarget) {_tryAttackCD = _tryAttackCooldown; return;}

        List<EnemyAttackData> GoodDistance = new List<EnemyAttackData>();
        foreach (EnemyAttackData data in _enemyAttacks)
        {
            if (data.Range >= _enemyCombatSystem.GetTargetDistance && data.TooClose <= _enemyCombatSystem.GetTargetDistance)
            {
                GoodDistance.Add(data);
            }
        }
        if (GoodDistance.Count > 0)
        {
            PerformAttack(GoodDistance[Random.Range(0, GoodDistance.Count)]);
        }
        else
        {
            _tryAttackCD = _tryAttackCooldown; return;
        }
    }

    protected virtual void PerformAttack(EnemyAttackData attack)
    {
        _tryAttackCD = attack.Duration;
    }

    public override void ResetAttacks()
    {
        _tryAttackCD = 0;
    }

    public class EnemyAttackData : AttackData
    {
        public float Range;
        public float TooClose;
        public float Duration;
    }
}
