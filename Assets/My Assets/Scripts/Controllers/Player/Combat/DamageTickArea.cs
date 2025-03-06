using UnityEngine;

public class DamageTickArea : Damage
{
    private void OnTriggerStay(Collider other)
    {
        // Check if the owner can damage this object.
        if (_owner != null && !_owner.CanDamage(other.gameObject))
            return;

        // Determine the closest point on the other collider relative to this trigger's center.
        Vector3 closestPoint = other.ClosestPoint(transform.position);

        // Retrieve the damageable component from the object.
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            // Apply full damage as long as the object is inside the trigger.
            float damage = _currentDamage * Time.deltaTime;
            Vector2 knockback = _knockback * Time.deltaTime;
            float knockout = _knockout * Time.deltaTime;

            damageableObject.TakeDamage(damage, _owner);
            damageableObject.TakeKnock(knockback, knockout, closestPoint);

            if (_acid > 0)
                damageableObject.TakeAcidDamage(_acid * Time.deltaTime);
            if (_fire > 0)
                damageableObject.TakeFireDamage(_fire * Time.deltaTime);
        }
    }
}
