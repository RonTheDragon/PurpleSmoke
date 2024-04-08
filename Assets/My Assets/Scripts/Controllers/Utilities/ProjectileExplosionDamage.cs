using UnityEngine;

public class ProjectileExplosionDamage : ExplosionDamage
{
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
}
