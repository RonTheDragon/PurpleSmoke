using UnityEngine;

public class TriggerDamage : Damage
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider belongs to the owner object
        if (other.gameObject == _owner)
        {
            return; // Prevent damaging the owner itself
        }

        // Check if the other collider belongs to a damageable object
        IDamageable damageableObject = other.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            // Apply damage and knockback to the damageable object
            damageableObject.TakeDamage(_currentDamage, _knockback, transform.position, _owner);
        }
    }
}
