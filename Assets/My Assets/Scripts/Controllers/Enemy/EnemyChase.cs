using UnityEngine;

public class EnemyChase : MonoBehaviour, IEnemyComponent
{
    private EnemyComponentRefrences _enemyComponents;
    private EnemyDetection _enemyDetection;
    private EnemyWalk _enemyWalk; 
    private Transform _target;
    private EnemyCombatSystem _enemyCombatSystem;

    [SerializeField] private float _loseTargetDuration = 2f; // Time to lose target
    private float _timeSinceLastSeen;
    [SerializeField] private float _giveUpIfThatFarAway;

    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _enemyComponents = EnemyComponents;
        _enemyDetection = _enemyComponents.GetEnemyDetection;
        _enemyWalk = _enemyComponents.GetEnemyWalk;
        _enemyCombatSystem = _enemyComponents.GetEnemyCombatSystem;

        _enemyDetection.OnTargetDetected += EnemyFound;
    }

    private void EnemyFound(Transform enemy)
    {
        _target = enemy;
        _enemyCombatSystem.SetTarget(enemy);
        _timeSinceLastSeen = 0f; // Reset timer when target is detected
        _enemyComponents.OnUpdate += EnemyUpdate;
    }

    private void EnemyUpdate()
    {
        CheckIfTargetStillVisible();
        SetWalkToTarget();
    }

    private void CheckIfTargetStillVisible()
    {
        if (_target != null)
        {
            // Check if the target is still visible
            if (_enemyDetection.TargetIsVisible(_target, _giveUpIfThatFarAway)) // Implement this method in EnemyDetection
            {
                _timeSinceLastSeen = 0f; // Reset timer
            }
            else
            {
                _timeSinceLastSeen += Time.deltaTime; // Increment timer
                if (_timeSinceLastSeen >= _loseTargetDuration)
                {
                    LoseTarget(); // Call lose target method if too much time has passed
                }
            }
        }
        else
        {
            LoseTarget();
        }
    }

    public void Agro()
    {
        _timeSinceLastSeen = 0f;
    }

    private void SetWalkToTarget()
    {
        if (_target != null)
        {
            _enemyWalk.SetDestination(_target.position);
        }
    }

    private void LoseTarget()
    {
        _enemyComponents.OnUpdate -= EnemyUpdate;
        _enemyDetection.ResetDetection();
        _target = null; // Clear the target
        _enemyCombatSystem.LoseTarget();
        _enemyWalk.StopMovement(); // Implement this method in EnemyWalk to stop moving

    }

    public Transform GetTarget => _target;
}
