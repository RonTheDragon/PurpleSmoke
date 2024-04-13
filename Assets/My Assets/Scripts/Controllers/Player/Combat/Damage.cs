using UnityEngine;

public class Damage : MonoBehaviour
{
    protected float _currentDamage;
    protected Vector2 _knockback;
    protected float _knockout;
    protected GameObject _owner;
    protected float _acid;

    public void SetDamage(float damage)
    {
        _currentDamage = damage;
    }

    public void SetKnock(Vector2 knockback, float knockout)
    {
        _knockback = knockback;
        _knockout = knockout;
    }

    public void SetAcidDamage(float acid)
    {
        _acid = acid;
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }
}
