using System;
using UnityEngine;
using UnityEngine.VFX;

public abstract class ProjectileMovement : MonoBehaviour
{
    protected float _speed;
    protected float _gravity;
    [SerializeField] protected Transform _modelTransform;
    [SerializeField] protected VisualEffect _visualEffect;

    // Call this to set up the speed and gravity, and launch the projectile.
    public void SetVariablesAndLaunch(float speed, float gravity)
    {
        SetVariables(speed, gravity);
        Launch();
    }

    private void SetVariables(float speed, float gravity)
    {
        _speed = speed;
        _gravity = gravity;
    }

    // Abstract Launch method that will be implemented in child classes.
    protected abstract void Launch();

    // Abstract method for projectile updates that can be used in a loop.
    public abstract void ActivateLoop(ref Action loop);
}
