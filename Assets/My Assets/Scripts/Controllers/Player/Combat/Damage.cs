using UnityEngine;

public class Damage : MonoBehaviour
{
    protected float _currentDamage;
    protected Vector2 _knockback;
    protected float _knockout;
    protected GameObject _owner;

    public void SetDamage(float damage, Vector2 knockback,float knockout)
    {
        _currentDamage = damage;
        _knockback = knockback;
        _knockout = knockout;
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }
}
