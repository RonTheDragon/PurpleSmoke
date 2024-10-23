using UnityEngine;
using UnityEngine.AI;

public class EnemyComponentRefrences : ComponentsRefrences
{
    [SerializeField] private EnemyWalk _enemyWalk;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        InitializeComponents();
    }

    protected override void InitializeComponents()
    {
        _enemyWalk.InitializeEnemyComponent(this);
    }

    public NavMeshAgent GetNavMeshAgent => _navMeshAgent;
}
