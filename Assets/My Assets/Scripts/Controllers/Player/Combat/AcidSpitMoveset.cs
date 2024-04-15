using System;
using UnityEngine;

public class AcidSpitMoveset : ChargeableMoveSet
{
    private ProjectilePooler _projectilePooler;
    private Transform _shooter;
    private bool _spittingAcid;
    private PlayerAcidation _playerAcidation;

    [SerializeField] private AcidSpitAttack _acidSpitAttack;
    [SerializeField] private AcidShotgunAttack _acidShotgunAttack;

    public override void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        _projectilePooler = GameManager.Instance.ProjectilePooler;
        base.MoveSetStart(playerCombatSystem);
        _playerAnimations = playerCombatSystem.GetAnimations();
        _shooter = playerCombatSystem.GetShooter();
        _playerAcidation = playerCombatSystem.GetPlayerAcidation();
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

        if (!_playerAcidation.TrySpendAcid(_acidShotgunAttack.AcidCost)) return;

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

        if (_playerCombatSystem.GetAcidation()) 
        {
            _acidShotgunAttack.PelletAmount = (int)(_acidShotgunAttack.PelletAmount * _acidShotgunAttack.AcidationPelletMult);
            _acidShotgunAttack.Spread = _acidShotgunAttack.AcidationSpread;
            _acidShotgunAttack.TimeBeforeDissolve = _acidShotgunAttack.AcidationTimeBeforeDissolve;
        }
        else
        {
            _acidShotgunAttack.Spread = _acidShotgunAttack.NormalSpread;
            _acidShotgunAttack.TimeBeforeDissolve = _acidShotgunAttack.NormalTimeBeforeDissolve;
        }

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
            if (!_playerAcidation.TrySpendAcid(_acidSpitAttack.AcidCost)) return;
            PerformAcidSpit();
        }
    }

    private void PerformAcidSpit()
    {
        // _playerAnimations.PlayAnimation(_acidSpitAttack.AnimationName); //no animation yet
        _castTimeLeft = _playerCombatSystem.GetAcidation() ? _acidSpitAttack.AcidationCastTime : _acidSpitAttack.CastTime;
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
    public float AcidCost;
}

[Serializable]
public class AcidSpitAttack : AcidProjectile
{
    public float ProjectileGravity;
    public float ProjectileExplosionRange;
    public float AcidationCastTime;
    // Add other properties related to the acid spit attack if needed
}

[Serializable]
public class AcidShotgunAttack : AcidProjectile
{
    public ChargeableStats chargeableStats;
    public int MinPelletAmount;
    public int MaxPelletAmount;
    public float AcidationPelletMult;
    [HideInInspector] public int PelletAmount;
    [HideInInspector] public float TimeBeforeDissolve;
    public float NormalTimeBeforeDissolve;
    public float AcidationTimeBeforeDissolve;
    [HideInInspector] public float Spread;
    public float NormalSpread;
    public float AcidationSpread;
    public Vector2 Knockback;
    public float Knockout;
    // Add other properties related to the acid spit attack if needed
}