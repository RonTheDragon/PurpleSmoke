using UnityEngine;

public class EnemyRoam : MonoBehaviour, IEnemyComponent
{
    private EnemyComponentRefrences _enemyComponents;
    private EnemyDetection _enemyDetection;
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        _enemyComponents = EnemyComponents;
        _enemyDetection = _enemyComponents.GetEnemyDetection;
        _enemyComponents.OnUpdate += EnemyUpdate;
        _enemyDetection.OnTargetLost += () => _enemyComponents.OnUpdate += EnemyUpdate;
        _enemyDetection.OnTargetDetected += (t) => _enemyComponents.OnUpdate -= EnemyUpdate;
    } 

    private void EnemyUpdate()
    {

    }
}
