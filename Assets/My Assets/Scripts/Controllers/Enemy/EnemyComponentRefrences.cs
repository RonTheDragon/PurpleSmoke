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
    [SerializeField] private EnemyKnockback _enemyKnockback;
    [SerializeField] private EnemyKnockout _enemyKnockout;
    [SerializeField] private EnemyDeath _enemyDeath;
    [SerializeField] private EnemyAnimations _enemyAnimations;
    [SerializeField] private EnemyCombatSystem _enemyCombatSystem;
    [SerializeField] private CombatRules _combatRules;

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
        _enemyKnockback.InitializeEnemyComponent(this);
        _enemyKnockout.InitializeEnemyComponent(this);
        _enemyDeath.InitializeEnemyComponent(this);
        _enemyAnimations.InitializeEnemyComponent(this);
        _enemyCombatSystem.InitializeEnemyComponent(this);
    }

    public NavMeshAgent GetNavMeshAgent => _navMeshAgent;
    public EnemyWalk GetEnemyWalk => _enemyWalk;
    public EnemyDetection GetEnemyDetection => _enemyDetection;
    public EnemyRoam GetEnemyRoam => _enemyRoam;
    public EnemyChase GetEnemyChase => _enemyChase;
    public EnemyHealth GetEnemyHealth => _enemyHealth;
    public EnemyKnockback GetEnemyKnockback => _enemyKnockback;
    public EnemyKnockout GetEnemyKnockout => _enemyKnockout;
    public EnemyDeath GetEnemyDeath => _enemyDeath;
    public EnemyAnimations GetEnemyAnimations => _enemyAnimations;
    public EnemyCombatSystem GetEnemyCombatSystem => _enemyCombatSystem;
    public CombatRules GetCombatRules => _combatRules;
}
