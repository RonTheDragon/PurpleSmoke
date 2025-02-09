using UnityEngine;

public class UnarmedMoveset : MeleeMoveset
{
    [SerializeField] private LightAttack[] _lightAttacksMoving = new LightAttack[5];
    [SerializeField] private LightAttack[] _lightAttacksInPlace = new LightAttack[3];
    [SerializeField] private LightAttackWithMovement _lightAttackInAir;
    [SerializeField] private HeavyAttack _heavyAttackMoving;
    [SerializeField] private HeavyAttackWithMovement _heavyAttackInPlace;
    [SerializeField] private HeavyDownAttack _heavyDownAttack;

    private VEPooler _vePooler;
    
    [SerializeField] private ExplosionDamage _explosionDamage;
    

    public override void MoveSetStart(CombatSystem combatSystem)
    {
        base.MoveSetStart(combatSystem);
        _playerCombatSystem = (PlayerCombatSystem)combatSystem;
        _vePooler = GameManager.Instance.GetVEPooler;
        PlayerComponentsRefrences refs = _playerCombatSystem.GetPlayerRefs;
        _playerAnimations = refs.GetPlayerAnimations;
        _playerGroundCheck = refs.GetPlayerGroundCheck;
        _playerMovement = refs.GetPlayerWalk;
        _playerAttackMovement = refs.GetPlayerAttackMovement;
        _playerGravity = refs.GetPlayerGravity;
        _playerJump = refs.GetPlayerJump;
        _owner = refs.GetCombatRules;
    }

    protected override void LightMoving()
    {
        PerformLightAttack(_lightAttacksMoving[_currentCombo]);
        _currentCombo++;
        if (_currentCombo >= _lightAttacksMoving.Length)
        {
            BreakCombo();
        }
    }


    protected override void LightInPlace()
    {
        _playerMovement.AddNotMovingReason("Attack");
        PerformLightAttack(_lightAttacksInPlace[_currentCombo]);
        _currentCombo++;
        if (_currentCombo >= _lightAttacksInPlace.Length)
        {
            BreakCombo();
        }
    }
    protected override void LightInAir()
    {
        _playerMovement.AddNotMovingReason("Attack");
        _playerJump.StopJumpMidAir();
        _playerGravity.AddNotFallingReason("AirAttack");
        _attackedInAir = true;
        PerformLightAttack(_lightAttackInAir);
        _playerAttackMovement.SetMovement(_lightAttackInAir.Movement);
    }

    protected override void HeavyMoving()
    {
        PerformCharging(_heavyAttackMoving.ChargeableStats);
    }

    protected override void HeavyInPlace()
    {
        PerformCharging(_heavyAttackInPlace.ChargeableStats);
    }

    protected override void HeavyInAir()
    {
        PerformCharging(_heavyDownAttack.ChargeableStats);
    }

    public override void OnReleaseHeavyAttack()
    {
        base.OnReleaseHeavyAttack();
        if (_castTimeLeft > 0 || _currentCharge == 0 || _releasedEarly) return; //dismiss press

        if (CheckAndHandleEarlyRelease()) return; //released too early

        switch (_currentChargedAttack)
        {
            case 0:
                PerformHeavyAttack(_heavyAttackMoving);
                break;
            case 1:
                PerformHeavyAttack(_heavyAttackInPlace);
                _playerMovement.AddNotMovingReason("Attack");
                _playerMovement.RemoveSpeedModifier("AimingKick");
                break;
            case 2:
                PerformHeavyAttackDownExplosive(_heavyDownAttack);
                break;
            default:
                break;
        }

        ResetCharge();
        BreakCombo();
    }

    private void PerformHeavyAttackDownExplosive(HeavyDownAttack attack)
    {
        float chargePercentage = GetChargePercentage();
        float damage = Mathf.Lerp(attack.MinDamage, attack.MaxDamage, chargePercentage);
        Vector2 knockback = Vector2.Lerp(attack.MinKnockback, attack.MaxKnockback, chargePercentage);
        float radius = Mathf.Lerp(attack.MinRadius, attack.MaxRadius, chargePercentage);

        Vector2 knockoutChance = Vector2.Lerp(attack.MinKnockout, attack.MaxKnockout, chargePercentage);
        float knockout = Random.Range(knockoutChance.x, knockoutChance.y);

        _playerAnimations.PlayAnimation(attack.AnimationName);
        _castTimeLeft = attack.CastTime;

        if (_explosionDamage != null)
        {
            AddDamager(_explosionDamage);
            _explosionDamage.SetOwner(_owner);
            _explosionDamage.SetDamage(damage);
            _explosionDamage.SetKnock(knockback, knockout);
            _explosionDamage.SetRadius(radius);

            if (_playerCombatSystem.GetAcidation)
            {
                float Acid = Mathf.Lerp(attack.MinAcid, attack.MaxAcid, chargePercentage);
                _explosionDamage.SetAcidDamage(Acid);
                _explosionDamage.SetRadius(radius * attack.AcidationRadiusMultiplier);
            }
            else
            {
                _explosionDamage.SetAcidDamage(0);
            }
        }
        
        _playerAttackMovement.SetCrashingDownSpeed(attack.DownSpeed);
        _playerAttackMovement.CrashDown();
        _castTimeLeft = 100;
    }

    public void PerformExplosionDamage()
    {
        _explosionDamage?.Explode();
        if (_playerCombatSystem.GetAcidation)
        {
            _vePooler.SpawnFromPool(_heavyDownAttack.AcidationVE, transform.position, Quaternion.Euler(90,0,0)).PlayEffect();
        }
    }

    protected override void AttackEnds()
    {
        _playerMovement.RemoveSpeedModifier("AimingKick");
        base.AttackEnds();
    }

    private void OnCrashedDown()
    {
        _playerAnimations.PlayAnimation(_heavyDownAttack.CrashAnimationName);
        _castTimeLeft = _heavyDownAttack.CastTime;
        _attackedInAir = false;
    }

    protected override void OnEquip()
    {
        _playerGroundCheck.OnGroundCheckChange += OnGroundedChanged;
        _playerAttackMovement.OnCrashedDown += OnCrashedDown;
    }

    protected override void OnUnequip()
    {
        _playerGroundCheck.OnGroundCheckChange -= OnGroundedChanged;
        _playerAttackMovement.OnCrashedDown -= OnCrashedDown;
        _lightAttacking = false;
    }


    [System.Serializable]
    class HeavyDownAttack : HeavyAttack
    {
        public float DownSpeed, MinRadius, MaxRadius, AcidationRadiusMultiplier;
        public string CrashAnimationName, AcidationVE;
    }
}
