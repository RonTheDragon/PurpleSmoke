using UnityEngine;
using System.Collections.Generic;

public class ParticleDamage : Damage
{
    [SerializeField] private ParticleSystem _particleSystem;
    private void OnParticleCollision(GameObject other)
    {
        // Check if the other collider belongs to the owner object
        if (other.gameObject == _owner)
        {
            return; // Prevent damaging the owner itself
        }

        // Create a list to store collision events dynamically
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        // Get all collisions from the current particle system
        int numCollisions = _particleSystem.GetCollisionEvents(other, collisionEvents);

        // Iterate over each collision event
        for (int i = 0; i < numCollisions; i++)
        {
            // Retrieve the collision point and normal
            Vector3 collisionPoint = collisionEvents[i].intersection;

            // Check if the other collider belongs to a damageable object
            IDamageable damageableObject = other.GetComponent<IDamageable>();

            if (damageableObject != null)
            {
                // Apply damage and knockback to the damageable object
                damageableObject.TakeDamage(_currentDamage, _owner);
                damageableObject.TakeKnock(_knockback, _knockout, collisionPoint);
                if (_acid > 0)
                {
                    damageableObject.TakeAcidDamage(_acid);
                }
            }
        }
    }
}
