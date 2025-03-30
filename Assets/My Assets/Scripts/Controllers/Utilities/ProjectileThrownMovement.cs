using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrownMovement : ProjectileMovement
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    protected override void Launch()
    {
        // Apply force to the Rigidbody for the thrown projectile
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(transform.forward * _speed, ForceMode.Impulse);
            _rigidbody.mass = _gravity;
        }

        // Optional visual effect initialization if needed
        if (_visualEffect != null)
        {
            _visualEffect.Reinit();
        }

        if (_trailRenderer != null) 
        { 
            _trailRenderer.Clear();
        }
    }

    public override void ActivateLoop(ref Action loop)
    {
        // This could be empty or have specific logic for thrown projectiles.
    }
}
