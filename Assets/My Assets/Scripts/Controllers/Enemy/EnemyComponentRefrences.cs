using UnityEngine;
using UnityEngine.AI;

public class EnemyComponentRefrences : ComponentsRefrences
{
    [SerializeField] private EnemyWalk _enemyWalk;
    [SerializeField] private EnemyDetection _enemyDetection;
    [SerializeField] private EnemyRoam _enemyRoam;
    [SerializeField] private EnemyChase _enemyChase;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private EnemyDeath _enemyDeath;

    private void Start()
    {
        InitializeComponents();
    }

    protected override void InitializeComponents()
    {
        _enemyWalk.InitializeEnemyComponent(this);
        _enemyDetection.InitializeEnemyComponent(this);
        _enemyRoam.InitializeEnemyComponent(this);
        _enemyChase.InitializeEnemyComponent(this);
        _enemyHealth.InitializeEnemyComponent(this);
        _enemyDeath.InitializeEnemyComponent(this);
    }

    public NavMeshAgent GetNavMeshAgent => _navMeshAgent;
    public EnemyWalk GetEnemyWalk => _enemyWalk;
    public EnemyDetection GetEnemyDetection => _enemyDetection;
    public EnemyRoam GetEnemyRoam => _enemyRoam;
    public EnemyChase GetEnemyChase => _enemyChase;
    public EnemyHealth GetEnemyHealth => _enemyHealth;
    public EnemyDeath GetEnemyDeath => _enemyDeath;
}
