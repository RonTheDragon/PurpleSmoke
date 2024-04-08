using UnityEngine;
using UnityEngine.VFX;

public class ExplosionVE : VisualEffectHandler
{
    [SerializeField] private VisualEffect _explosionEffect;

    public override void PlayEffect()
    {
        _explosionEffect.Play();
    }
}
