using UnityEngine;

public class EnemyDeath : MonoBehaviour , IEnemyComponent
{
    public void InitializeEnemyComponent(EnemyComponentRefrences EnemyComponents)
    {
        
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
