using UnityEngine;

public class TriggerDamage : Damage
{
    private void OnTriggerEnter(Collider other)
    {
        if (_owner == null || !_owner.CanDamage(other.gameObject))
        {
            return; 
        }

        // Check if the other collider belongs to a damageable object
        IDamageable damageableObject = other.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            // Apply damage and knockback to the damageable object
            damageableObject.TakeDamage(_currentDamage, _owner);
            damageableObject.TakeKnock(_knockback, _knockout, _owner.transform.position);
            if (_acid > 0)
            {
                damageableObject.TakeAcidDamage(_acid);
            }
        }
    }
}
