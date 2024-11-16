using UnityEngine;

public class EnemyAttackMovement : CharacterAttackMovement, IEnemyComponent
{
    [SerializeField] private Transform _targetToJumpToward; // Target to jump towards
    private bool RotateToward = false; // Flag to check if enemy has rotated toward target
    private EnemyWalk _enemyWalk;

    // Initialize enemy components (called from another system)
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _characterController = enemyComponents.GetCharacterController;
        _characterBody = enemyComponents.GetEnemyBody;
        _enemyWalk = enemyComponents.GetEnemyWalk;
        

        enemyComponents.OnUpdate += EnemyUpdate;
    }

    // Update movement during each frame
    private void EnemyUpdate()
    {
        if (_characterController != null)
        {
            ApplyingMovement();
        }
        if (RotateToward)
        {
            RotatingTowardTarget();
        }
    }

    
    public void RotateToTarget()
    {
        _enemyWalk.AddNotNavmeshReason("attack");
        _enemyWalk.AddNotFallingReason("attack");
        _enemyWalk.AddNotMovingReason("attack");
        RotateToward = true;
    }

    // Method to rotate toward target (called by an animation event)
    public void RotatingTowardTarget()
    {
        if (_targetToJumpToward == null) return;

        // Calculate direction to target (excluding vertical direction for smooth horizontal rotation)
        Vector3 targetDirection = _targetToJumpToward.position - _characterBody.position;

        // Calculate target rotation based on the direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Smoothly rotate the character towards the target rotation
        _characterBody.rotation = Quaternion.RotateTowards(_characterBody.rotation, targetRotation, 1360 * Time.deltaTime);
    }


    public override void ApplyMovement()
    {
        base.ApplyMovement();
        RotateToward = false;
    }

    // Stop movement and reset states (called by an animation event)
    public override void StopMovement()
    {
        base.StopMovement();
        _enemyWalk.RemoveNotFallingReason("attack");
        _enemyWalk.RemoveNotNavmeshReason("attack");
        _enemyWalk.RemoveNotMovingReason("attack");
        _characterBody.rotation = Quaternion.Euler(0f, _characterBody.rotation.eulerAngles.y, 0f);
        RotateToward = false;
        ClearTarget();
    }

    // Clear target and reset state
    public void ClearTarget()
    {
        _targetToJumpToward = null;
    }

    // Set a new target for the enemy to jump towards
    public void SetTarget(Transform target)
    {
        _targetToJumpToward = target;
    }
}
