using UnityEngine;
using static CombatMoveSet;

public class ProjectileFlameExplosionDamage : ExplosionDamage
{
    [SerializeField] private LayerMask floorLayer; // Assign the floor layer in the Inspector
    [SerializeField] private float maxDistanceToFloor = 5f; // Max detection range
    private AttackData _attackdata;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner)
        {
            return; // Prevent damaging the owner itself
        }
        else
        {
            Explode();
            gameObject.SetActive(false);
        } 
    }

    public void SetAttackData(AttackData a)
    {
        _attackdata = a;
    }

    protected override void ExplosionEffect()
    {
        TrySpawnEffectOnFloor();
    }

    private void TrySpawnEffectOnFloor()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        // Shoot a raycast straight down
        if (Physics.Raycast(origin, direction, out hit, maxDistanceToFloor, floorLayer))
        {
            // Get the hit point on the floor
            Vector3 spawnPosition = hit.point;

            // Spawn the visual effect
            ExplosiveFlameVE ve = (ExplosiveFlameVE)GameManager.Instance.GetVEPooler.SpawnFromPool(_explosionTag, spawnPosition, Quaternion.identity);
            ve.PlayEffect();
            ve.SetDamageTickArea(_owner, _attackdata);
        }
    }

}
