using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : CharacterWalk, IEnemyComponent
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _destination;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _characterController = EnemyComponents.GetCharacterController;
        _navMeshAgent = EnemyComponents.GetNavMeshAgent;
        EnemyComponents.OnUpdate += EnemyUpdate;
    }       

    private void EnemyUpdate()
    {
        _navMeshAgent.SetDestination(_destination.position);
    }
}
