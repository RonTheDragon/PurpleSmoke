using System;
using UnityEngine;
using UnityEngine.VFX;

public class ProjectileMovement : MonoBehaviour
{
    private float _speed;
    private float _gravity;
    [SerializeField] Transform _modelTransform;
    [SerializeField] VisualEffect _visualEffect;

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

    private void Launch()
    {
        float randomRotation = UnityEngine.Random.Range(0f, 360f);
        _modelTransform.Rotate(randomRotation, 0f, 0f);
        _visualEffect.Reinit();
        transform.Rotate(Vector3.left, 10);
    }

    public void ActivateLoop(ref Action loop)
    {
            loop += ProjectileUpdate;
    }


    private void ProjectileUpdate()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;

        if (transform.rotation.x < 0.6f) { transform.Rotate(Vector3.left, -_gravity * Time.deltaTime); }
    }

}
