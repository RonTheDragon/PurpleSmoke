using System;
using UnityEngine;

public class MovingProjectile : Projectile
{
    [SerializeField] protected ProjectileMovement _movement;
    private bool _loopStarted;
    private Action _loop;

    public override void SetProjectile(CombatRules owner, ProjectileAttack projectileAttack)
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
