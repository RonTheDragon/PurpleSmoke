using UnityEngine;
using UnityEngine.VFX;
using static ClownMoveSet;
using static CombatMoveSet;

public class ExplosiveFlameVE : ExplosionVE
{
    [SerializeField] private DamageTickArea tickArea;
    [SerializeField] private VisualEffect _acidExplosion;
    public override void PlayEffect()
    {
        base.PlayEffect();
    }

    public void SetDamageTickArea(CombatRules owner,AttackData attackData)
    {
        if (attackData is BallThrow)
        {
            BallThrow ball = (BallThrow)attackData;
            tickArea.SetOwner(owner);
            tickArea.SetDamage(ball.GroundFire_Damage);
            tickArea.SetFireDamage(ball.GroundFire_Fire);
            Invoke(nameof(Disappear), ball.GroundFire_Time);
            if (ball.AcidUsed)
            {
                _acidExplosion.Play();
            }
            else
            {
                _acidExplosion.Stop();
            }
        }
    }

    private void Disappear()
    {
        gameObject.SetActive(false);
    }
}
