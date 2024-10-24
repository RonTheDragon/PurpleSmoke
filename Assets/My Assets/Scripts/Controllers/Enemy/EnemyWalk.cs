using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : CharacterWalk, IEnemyComponent
{
    private NavMeshAgent _navMeshAgent;
    private Vector3 _destination;

    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _characterController = EnemyComponents.GetCharacterController;
        _navMeshAgent = EnemyComponents.GetNavMeshAgent;
        EnemyComponents.OnUpdate += EnemyUpdate;
    }

    public void SetDestination(Vector3 destination) 
    {
        _destination = destination;
    }

    public void StopMovement()
    {
        _destination = transform.position;
    }

    private void EnemyUpdate()
    {
        _navMeshAgent.SetDestination(_destination);
    }
}
