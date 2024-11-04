using UnityEngine;

public class EnemyCombatSystem : CombatSystem, IEnemyComponent
{
    EnemyComponentRefrences _enemyComponents;
    [SerializeField] private EnemyCombatMoveSet _moveSet;
    private Transform _target;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _enemyComponents = enemyComponents;
        _enemyComponents.OnUpdate += EnemyUpdate;
        _moveSet.MoveSetStart(this);
    }

    private void EnemyUpdate()
    {
        _moveSet.MoveSetUpdate();
    }

    public Transform GetTarget => _target;

    public bool HasTarget => _target != null;

    public float GetTargetDistance => Vector3.Distance(_target.position,transform.position);

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void LoseTarget()
    {
        _target = null;
    }
}
