using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileMovement _movement;
    private bool _loopStarted;
    private Action _loop;

    public void SetProjectile(float _speed, float _gravity)
    {
        StartLoops();
        _movement.SetVariablesAndLaunch(_speed, _gravity);
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
