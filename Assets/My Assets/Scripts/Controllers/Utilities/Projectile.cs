using UnityEngine;
using static CombatMoveSet;

public abstract class Projectile : MonoBehaviour
{
    public abstract void SetProjectile(CombatRules owner, AttackData projectileAttack);
}
