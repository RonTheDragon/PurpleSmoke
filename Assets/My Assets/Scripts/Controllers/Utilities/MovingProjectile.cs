using System;
using UnityEngine;
using static CombatMoveSet;

public class MovingProjectile : Projectile
{
    [SerializeField] protected ProjectileMovement _movement;
    private bool _loopStarted;
    private Action _loop;

    public override void SetProjectile(CombatRules owner, AttackData projectileAttack)
    {
        StartLoops();
    }

    private void StartLoops()
    {
        if (!_loopStarted)
        {
            _loopStarted = true;
            _movement.ActivateLoop(ref _loop);
        }
    }

    private void Update()
    {
        _loop?.Invoke();
    }
}
