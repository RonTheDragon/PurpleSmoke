using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : CharacterWalk, IEnemyComponent
{
    private NavMeshAgent _navMeshAgent;
    private EnemyAnimations _enemyAnimations;
    private Vector3 _destination;
    [ReadOnly][SerializeField] private List<string> _notNavmeshReasons = new List<string>();
    private bool _tryToNavmesh;

    // New gravity variables
    [SerializeField] private float _gravity = -9.81f; // Gravity force
    private Vector3 _previousLocation;
    [SerializeField] private float _movementCheckCooldown = 0.5f;
    private float _originalStepOffset;

    [SerializeField] private float _destinationUpdateInterval = 0.5f; // Time interval in seconds
    private float _nextDestinationUpdateTime;

    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _characterController = EnemyComponents.GetCharacterController;
        _navMeshAgent = EnemyComponents.GetNavMeshAgent;
        _enemyAnimations = EnemyComponents.GetEnemyAnimations;
        _currentSpeed = _baseWalkingSpeed;
        _navMeshAgent.speed = _currentSpeed;
        _navMeshAgent.angularSpeed = _currentTurnSpeed;
        EnemyComponents.OnUpdate += EnemyUpdate;
        InvokeRepeating(nameof(CheckIfMoving), 0, _movementCheckCooldown);
        _previousLocation = transform.position;
        _originalStepOffset = _characterController.stepOffset;
    }

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    public void StopMovement()
    {
        _destination = transform.position;
    }

    private void CheckIfMoving()
    {
        if (!_canMove) { _enemyAnimations.ChangeWalk(0); }
        else
        {
            int i = Vector3.Distance(_previousLocation, transform.position) > 0.1f ? 1 : 0;
            _enemyAnimations.ChangeWalk(i);
            _previousLocation = transform.position;
        }
    }

    private void EnemyUpdate()
    {
        if (_navMeshAgent.enabled == false)
        {
            Gravity();
        }

        if (!_canMove) return;

        if (_navMeshAgent.enabled == true)
        {
            if (Time.time >= _nextDestinationUpdateTime)
            {
                _navMeshAgent.SetDestination(_destination);
                _nextDestinationUpdateTime = Time.time + _destinationUpdateInterval;
            }
            if (Vector3.Distance(transform.position, _destination) < _navMeshAgent.stoppingDistance)
            {
                RotateTowardDestination();
            }
        }
        else if (_tryToNavmesh)
        {
            TryToNavmesh();
        }

        if (_navMeshAgent.enabled == false)
        {
            MovementWithoutNavmesh();
        }
    }

    private void MovementWithoutNavmesh()
    {
        RotateTowardDestination();

        if (Vector3.Distance(transform.position, _destination) < 0.1f)
        {
            return;
        }
        // Move the character controller forward
        _characterController.Move(transform.forward * _currentSpeed * Time.deltaTime);
    }

    private void RotateTowardDestination()
    {
        Vector3 direction = new Vector3(_destination.x, transform.position.y, _destination.z) - transform.position;
        direction.y = 0; // Keep the movement in the XZ plane
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _currentTurnSpeed * Time.deltaTime);
    }

    private void Gravity()
    {
        _characterController.Move(Vector3.down * -_gravity * Time.deltaTime); // Apply gravity movement
    }

    private void TryToNavmesh()
    {
        // Check if the agent is close enough to the NavMesh
        NavMeshHit hit;
        float maxDistance = 2f; // Set the maximum distance to check for the NavMesh
        if (NavMesh.SamplePosition(transform.position, out hit, maxDistance, NavMesh.AllAreas))
        {
            _navMeshAgent.enabled = true;
            _characterController.stepOffset = 0;
        }
    }

    public void AddNotNavmeshReason(string reason)
    {
        if (!_notNavmeshReasons.Contains(reason))
        {
            _notNavmeshReasons.Add(reason);
            _tryToNavmesh = false;
            if (_navMeshAgent.enabled == true)
            {
                _navMeshAgent.enabled = false;
                _characterController.stepOffset = _originalStepOffset;
            }
        }
    }

    public void RemoveNotNavmeshReason(string reason)
    {
        if (_notNavmeshReasons.Contains(reason))
        {
            _notNavmeshReasons.Remove(reason);
        }
        if (_notNavmeshReasons.Count == 0)
        {
            _tryToNavmesh = true;
        }
    }
}
