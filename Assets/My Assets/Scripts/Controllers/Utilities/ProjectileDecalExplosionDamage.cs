using UnityEngine;

public class ProjectileDecalExplosionDamage : ExplosionDamage
{
    private Collider _colliderHit;
    private Transform _parentOfDecal;
    private Vector3 _hitPoint; // Store the hit point
    private Vector3 _hitNormal; // Store the surface normal

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner.gameObject || other.isTrigger)
        {
            return; // Prevent damaging the owner itself
        }
        else
        {
            _colliderHit = other;
            _parentOfDecal = null;
            if (other.gameObject.tag == "Player") { _parentOfDecal = other.transform.GetChild(0); }
            if (other.gameObject.tag == "Enemy") { _parentOfDecal = other.transform; }

            // Cast a ray to get the exact hit point and normal
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f, ~0, QueryTriggerInteraction.Collide))
            {
                _hitPoint = hit.point; // Store the precise hit location
                _hitNormal = hit.normal; // Store the surface normal
            }
            else
            {
                // Fallback: Use the collider's closest point and assume up as normal
                _hitPoint = _colliderHit.ClosestPoint(transform.position);
                _hitNormal = Vector3.up; // Default to up if raycast fails
            }

            Explode();
            gameObject.SetActive(false);
        }
    }

    protected override void ExplosionEffect()
    {
        // Use the inverted hit normal to flip the decal 180 degrees
        Quaternion rotation = Quaternion.LookRotation(-_hitNormal); // Flip by using the opposite direction

        // Generate a random angle for the Z rotation (in the plane of the surface)
        float randomZRotation = Random.Range(0f, 360f);

        // Apply the random Z rotation
        rotation *= Quaternion.Euler(0f, 0f, randomZRotation);

        // Spawn the visual effect at the stored hit point
        VisualEffectHandler ve = GameManager.Instance.GetVEPooler.SpawnFromPool(_explosionTag, _hitPoint, rotation);
        ve.transform.SetParent(_parentOfDecal);
        ve.PlayEffect();
    }
}