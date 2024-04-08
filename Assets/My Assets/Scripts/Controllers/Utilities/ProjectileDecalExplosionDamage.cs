using UnityEngine;

public class ProjectileDecalExplosionDamage : ExplosionDamage
{
    private Collider _colliderHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner)
        {
            return; // Prevent damaging the owner itself
        }
        else
        {
            _colliderHit = other;
            Explode();
            gameObject.SetActive(false);
        }
    }

    protected override void ExplosionEffect()
    {
        // Calculate the closest point on the collider to the projectile's position
        Vector3 closestPoint = _colliderHit.ClosestPointOnBounds(transform.position);

        // Calculate the direction between the explosion point and the closest point on the collider
        Vector3 direction = closestPoint - transform.position;

        Quaternion rotation = new Quaternion();
        // Calculate the rotation based on the direction
        if (direction != Vector3.zero)
        {
             rotation = Quaternion.LookRotation(direction);
        }

        // Generate a random angle for the Z rotation
        float randomZRotation = Random.Range(0f, 360f);

        // Apply the random Z rotation to the rotation
        rotation *= Quaternion.Euler(0f, 0f, randomZRotation);

        // Spawn the visual effect
        VisualEffectHandler ve = GameManager.Instance.VisualEffectsPooler.SpawnFromPool(_explosionTag, transform.position, rotation);
        ve.PlayEffect();
    }

}
