using UnityEngine;

public class AcidSpitMoveset : ChargeableMoveSet
{
    private ProjectilePooler _projectilePooler;
    private Transform _shooter;
    private bool _spittingAcid;

    [SerializeField] private AcidSpitAttack _acidSpitAttack;
    [SerializeField] private AcidShotgunAttack _acidShotgunAttack;

    public override void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        _projectilePooler = GameManager.Instance.ProjectilePooler;
        base.MoveSetStart(playerCombatSystem);
        _playerAnimations = playerCombatSystem.GetAnimations();
        _shooter = playerCombatSystem.GetShooter();
    }

    public override void OnLightAttack()
    {
        _spittingAcid = true;
    }

    public override void OnReleaseLightAttack()
    {
        _spittingAcid = false;
    }

    public override void OnHeavyAttack()
    {
        if (_castTimeLeft > 0 || _releasedEarly) return;
        _playerCombatSystem.SetBusyAttacking(true);

        PerformCharging(_acidShotgunAttack.chargeableStats);
        base.OnHeavyAttack();
    }

    public override void OnReleaseHeavyAttack()
    {
        if (_castTimeLeft > 0 || _currentCharge == 0 || _releasedEarly) return; //dismiss press

        if (CheckAndHandleEarlyRelease()) return; //released too early

        PerformAcidShotgun();

        ResetCharge();
    }

    private void PerformAcidShotgun()
    {
        float chargePercentage = GetChargePercentage();
        _acidShotgunAttack.PelletAmount = (int)Mathf.Lerp(_acidShotgunAttack.MinPelletAmount, _acidShotgunAttack.MaxPelletAmount, chargePercentage);

        _castTimeLeft = _acidSpitAttack.CastTime;
        Projectile projectile = _projectilePooler.SpawnFromPool(_acidShotgunAttack.ProjectileTag, _shooter.position, _shooter.rotation);

        projectile.SetProjectile(transform.parent.gameObject, _acidShotgunAttack);
    }

    public override void MoveSetUpdate()
    {
        base.MoveSetUpdate();

        if (_spittingAcid)
        {
            TrySpitAcid();
        }

        if (_castTimeLeft > 0)
        {
            _castTimeLeft -= Time.deltaTime;
        }
        else if (_castTimeLeft < 0)
        {
            _castTimeLeft = 0;
            _playerCombatSystem.SetBusyAttacking(false);
        }
    }

    private void TrySpitAcid()
    {
        if (!_isCharging && _castTimeLeft <= 0)
        {
            PerformAcidSpit();
        }
    }

    private void PerformAcidSpit()
    {
        // _playerAnimations.PlayAnimation(_acidSpitAttack.AnimationName); //no animation yet
        _castTimeLeft = _acidSpitAttack.CastTime;
        Projectile projectile = _projectilePooler.SpawnFromPool(_acidSpitAttack.ProjectileTag, _shooter.position, _shooter.rotation);

        projectile.SetProjectile(transform.parent.gameObject, _acidSpitAttack);
        // Apply other effects of the acid spit attack
        // You can modify this part based on your game logic
    }

    public override void ResetAttacks()
    {
        base.ResetAttacks();
        _spittingAcid = false;
    }

}

public class ProjectileAttack
{
    public string ProjectileTag;
    public string AnimationName;
    public float CastTime;
    public float ProjectileSpeed;
    public float ProjectileDamage;
}

public class AcidProjectile : ProjectileAttack
{
    public float ProjectileAcidDamage;
}

[System.Serializable]
public class AcidSpitAttack : AcidProjectile
{
    public float ProjectileGravity;
    public float ProjectileExplosionRange;
    // Add other properties related to the acid spit attack if needed
}

[System.Serializable]
public class AcidShotgunAttack : AcidProjectile
{
    public ChargeableStats chargeableStats;
    public int MinPelletAmount;
    public int MaxPelletAmount;
    [HideInInspector] public int PelletAmount;
    public float TimeBeforeDissolve;
    public float Spread;
    public Vector2 Knockback;
    public float Knockout;
    // Add other properties related to the acid spit attack if needed
}