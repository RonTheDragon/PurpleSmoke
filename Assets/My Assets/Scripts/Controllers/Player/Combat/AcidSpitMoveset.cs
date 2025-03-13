using System;
using UnityEngine;
using static CombatMoveSet;

public class AcidSpitMoveset : ChargeableMoveSet
{
    private ProjectilePooler _projectilePooler;
    private CombatRules _owner;
    private Transform _shooter;
    private bool _spittingAcid;
    private PlayerAcidation _playerAcidation;

    [SerializeField] private AcidSpitAttack _acidSpitAttack;
    [SerializeField] private AcidShotgunAttack _acidShotgunAttack;

    public override void MoveSetStart(CombatSystem combatSystem)
    {
        PlayerCombatSystem playerCombatSystem = (PlayerCombatSystem)combatSystem;
        _projectilePooler = GameManager.Instance.GetProjectilePooler;
        _owner = playerCombatSystem.GetPlayerRefs.GetCombatRules;
        base.MoveSetStart(playerCombatSystem);
        PlayerComponentsRefrences refs = playerCombatSystem.GetPlayerRefs;
        _playerAnimations = refs.GetPlayerAnimations;
        _shooter = refs.GetShooter;
        _playerAcidation = refs.GetPlayerAcidation;
        _playerCharging = refs.GetPlayerCharging;
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
        if (_castTimeLeft > 0 || _playerCharging.GetReleasedEarly) return;

        if (!_playerAcidation.TrySpendAcid(_acidShotgunAttack.AcidCost)) return;

        _playerCombatSystem.SetBusyAttacking(true);
        _playerCharging.PerformCharging(_acidShotgunAttack.chargeableStats, OnReleaseHeavyAttack);
        base.OnHeavyAttack();
    }

    public override void OnReleaseHeavyAttack()
    {
        if (_castTimeLeft > 0 ) return; //dismiss press
        if (_playerCharging.CheckAndHandleEarlyRelease()) return; //released too early


        PerformAcidShotgun();

        _playerCharging.ResetCharge(this);
    }

    private void PerformAcidShotgun()
    {
        float chargePercentage = _playerCharging.GetChargePercentage();
        _acidShotgunAttack.PelletAmount = (int)Mathf.Lerp(_acidShotgunAttack.MinPelletAmount, _acidShotgunAttack.MaxPelletAmount, chargePercentage);

        if (_playerCombatSystem.GetAcidation) 
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

        projectile.SetProjectile(_owner, _acidShotgunAttack);
        _playerAnimations.PlayAnimation(_acidShotgunAttack.AnimationName);
    }

    public override void MoveSetUpdate()
    {
       // base.MoveSetUpdate();


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
            AttackEnds();
        }
    }

    public override void ResetAttacks()
    {
        base.ResetAttacks();
        _playerAnimations.PlayAnimation("Cancel");
        AttackEnds();
    }

    private void AttackEnds()
    {
        _castTimeLeft = 0;
        _playerCombatSystem.SetBusyAttacking(false);
        _spittingAcid = false;
        _playerCharging.ResetCharge(this);
    }

    private void TrySpitAcid()
    {
        if (!_playerCharging.GetIsCharging && _castTimeLeft <= 0)
        {
            if (!_playerAcidation.TrySpendAcid(_acidSpitAttack.AcidCost)) return;
            PerformAcidSpit();
        }
    }

    private void PerformAcidSpit()
    {
        // _playerAnimations.PlayAnimation(_acidSpitAttack.AnimationName); //no animation yet
        _castTimeLeft = _playerCombatSystem.GetAcidation ? _acidSpitAttack.AcidationCastTime : _acidSpitAttack.CastTime;
        Projectile projectile = _projectilePooler.SpawnFromPool(_acidSpitAttack.ProjectileTag, _shooter.position, _shooter.rotation);

        projectile.SetProjectile(_owner, _acidSpitAttack);
        _playerAnimations.PlayAnimation(_acidSpitAttack.AnimationName);
        // Apply other effects of the acid spit attack
        // You can modify this part based on your game logic
    }

    protected override void OnEquip()
    {
        
    }

    protected override void OnUnequip()
    {
        
    }
}

public class ProjectileAttack : AttackData
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
    public Vector2 Knockback;
    public Vector2 Knockout;
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
    // Add other properties related to the acid spit attack if needed
}