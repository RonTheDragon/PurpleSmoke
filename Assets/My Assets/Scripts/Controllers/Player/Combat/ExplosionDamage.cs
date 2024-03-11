using UnityEngine;

public class ExplosionDamage : Damage
{
    protected float _radius;

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider col in colliders)
        {
            // Check if the collider belongs to the owner object
            if (col.gameObject == _owner)
            {
                continue; // Prevent damaging the owner itself
            }

            // Calculate distance from the explosion center
            float distance = Vector3.Distance(transform.position, col.transform.position);

            // Calculate damage and knockback based on distance from the explosion center
            float normalizedDistance = 1 - Mathf.Clamp01(distance / _radius); // Normalized distance from 0 to 1
            float damageMultiplier = normalizedDistance; // Damage multiplier decreases as distance increases
            float knockbackMultiplier = normalizedDistance; // Knockback multiplier decreases as distance increases

            // Apply damage and knockback to the damageable object
            IDamageable damageableObject = col.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                float damage = _currentDamage * damageMultiplier;
                Vector2 knockback = _knockback * knockbackMultiplier;
                damageableObject.TakeDamage(damage, knockback, transform.position, _owner);
            }
        }
    }
}
