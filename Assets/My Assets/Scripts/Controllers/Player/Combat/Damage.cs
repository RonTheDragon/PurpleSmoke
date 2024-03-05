using UnityEngine;

public class Damage : MonoBehaviour
{
    protected float _currentDamage;
    protected Vector2 _knockback;
    protected GameObject _owner;

    public void SetDamage(float damage, Vector2 knockback)
    {
        _currentDamage = damage;
        _knockback = knockback;
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }
}
