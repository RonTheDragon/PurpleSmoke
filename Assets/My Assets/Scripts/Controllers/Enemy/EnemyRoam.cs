using UnityEngine;

public class EnemyRoam : MonoBehaviour, IEnemyComponent
{
    private EnemyComponentRefrences _enemyComponents;
    private EnemyWalk _enemyWalk;
    private EnemyDetection _enemyDetection;
    [SerializeField] private float _roamCooldownMin, _roamCooldownMax;
    private float _roamCD;
    [SerializeField] private float _roamRadius;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _enemyComponents = EnemyComponents;
        _enemyDetection = _enemyComponents.GetEnemyDetection;
        _enemyWalk = _enemyComponents.GetEnemyWalk;
        _enemyComponents.OnUpdate += EnemyUpdate;
        _enemyDetection.OnTargetLost += () => _enemyComponents.OnUpdate += EnemyUpdate;
        _enemyDetection.OnTargetDetected += (t) => _enemyComponents.OnUpdate -= EnemyUpdate;
    } 

    private void EnemyUpdate()
    {
        RoamCooldown();
    }

    private void RoamCooldown()
    {
        if (_roamCD > 0) { _roamCD -= Time.deltaTime; }
        else
        {
            ChangeRandomDestination();
            _roamCD = Random.Range(_roamCooldownMin, _roamCooldownMax);
        }
    }

    private void ChangeRandomDestination()
    {
        float angle = Random.Range(0f, 2 * Mathf.PI); // Random angle in radians
        float distance = Random.Range(0, _roamRadius); // Random distance from the center

        // Calculate the new X and Z positions based on the angle and distance
        float randomX = Mathf.Cos(angle) * distance;
        float randomZ = Mathf.Sin(angle) * distance;

        // Create the new destination position
        Vector3 newDestination = new Vector3(
            transform.position.x + randomX,
            transform.position.y, // Keep the Y position unchanged
            transform.position.z + randomZ
        );

        // Set the destination for the enemy to roam to
        _enemyWalk.SetDestination(newDestination);
    }

}
