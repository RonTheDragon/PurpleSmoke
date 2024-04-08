using UnityEngine;

public class ExplosionDamage : Damage
{
    protected float _radius;
    protected string _explosionTag;

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    public virtual void Explode()
    {
        ExplosionEffect();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider col in colliders)
        {
            // Check if the collider belongs to the owner object
            if (col.gameObject == _owner)
            {
                continue; // Prevent damaging the owner itself
            }

            // Get the closest point on the collider's surface to the explosion center
            Vector3 closestPoint = col.ClosestPoint(transform.position);

            // Calculate distance from the explosion center using the closest point
            float distance = Vector3.Distance(transform.position, closestPoint);

            // Calculate damage and knockback based on distance from the closest point
            float normalizedDistance = 1 - Mathf.Clamp01(distance / _radius); // Normalized distance from 0 to 1

            // Apply damage and knockback to the damageable object
            IDamageable damageableObject = col.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                float damage = _currentDamage * normalizedDistance;
                Vector2 knockback = _knockback * normalizedDistance;
                float knockout = _knockout * normalizedDistance;
                damageableObject.TakeDamage(damage, knockback, knockout, closestPoint, _owner);

                if (_acid > 0)
                {
                    damageableObject.TakeAcidDamage(_acid * normalizedDistance);
                }
            }
        }
    }

    protected virtual void ExplosionEffect() 
    { 
        
    }

}
