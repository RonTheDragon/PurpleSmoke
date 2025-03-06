using UnityEngine;

public class Damage : MonoBehaviour
{
    protected float _currentDamage;
    protected Vector2 _knockback;
    protected float _knockout;
    protected CombatRules _owner;
    protected float _acid;
    protected float _fire;

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
    public void SetFireDamage(float fire)
    {
        _fire = fire;
    }

    public void SetOwner(CombatRules owner)
    {
        if (_owner != owner)
        _owner = owner;
    }
}
