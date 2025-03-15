using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStaticMovement : ProjectileMovement
{
    private bool _isMoving = true;

    protected override void Launch()
    {
        // Optional visual effect initialization if needed
        if (_visualEffect != null)
        {
            _visualEffect.Reinit();
        }
    }

    // Implement the movement loop (without using Update)
    public override void ActivateLoop(ref Action loop)
    {
        loop += ProjectileUpdate;
    }

    // Loop method for updating the position manually, including gravity
    private void ProjectileUpdate()
    {
        if (_isMoving)
        {
            // Move the projectile forward using its speed
            transform.position += transform.forward * _speed * Time.deltaTime;

            // Apply gravity to the projectile's rotation
            if (transform.rotation.x < 0.6f)
            {
                transform.Rotate(Vector3.left, -_gravity * Time.deltaTime); // Gravity is applied here
            }

            // Example condition to stop movement when hitting the ground
            if (transform.position.y < 0f)
            {
                _isMoving = false;
            }
        }
    }
}
