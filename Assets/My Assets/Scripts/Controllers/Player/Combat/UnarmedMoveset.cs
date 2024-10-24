using System.Collections.Generic;
using UnityEngine;

public class UnarmedMoveset : ChargeableMoveSet
{
    [SerializeField] private LightAttack[] _lightAttacksMoving = new LightAttack[5];
    [SerializeField] private LightAttack[] _lightAttacksInPlace = new LightAttack[3];
    [SerializeField] private LightAttackWithMovement _lightAttackInAir;
    [SerializeField] private HeavyAttack _heavyAttackMoving;
    [SerializeField] private HeavyAttackWithMovement _heavyAttackInPlace;
    [SerializeField] private HeavyDownAttack _heavyDownAttack;

    private PlayerGroundCheck _playerGroundCheck;
    private PlayerWalk _playerMovement;
    private PlayerAttackMovement _playerAttackMovement;
    private PlayerGravity _playerGravity;
    private PlayerJump _playerJump;
    private VEPooler _vePooler;
    [ReadOnly][SerializeField] private int _currentCombo;
    [SerializeField] private float _comboBreakTime;
    private float _comboTimeLeft;
    [SerializeField] private ExplosionDamage _explosionDamage;
    private int _lastAttackType;
    private bool _attackedInAir;
    private int _currentChargedAttack;
    private GameObject _owner;
    private List<Damage> _currentDamagers = new List<Damage>();
    private bool _lightAttacking;
    public override void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        base.MoveSetStart(playerCombatSystem);
        _vePooler = GameManager.Instance.GetVEPooler;
        PlayerComponentsRefrences refs = playerCombatSystem.GetPlayerRefs;
        _playerAnimations = refs.GetPlayerAnimations;
        _playerGroundCheck = refs.GetPlayerGroundCheck;
        _playerMovement = refs.GetPlayerWalk;
        _playerAttackMovement = refs.GetPlayerAttackMovement;
        _playerGravity = refs.GetPlayerGravity;
        _playerJump = refs.GetPlayerJump;
        _owner = transform.parent.gameObject;
    }

    public override void SubscribeToEvents()
    {
        UnsubscribeToEvents();
        _playerGroundCheck.OnGroundCheckChange += OnGroundedChanged;
        _playerAttackMovement.OnCrashedDown += OnCrashedDown;
    }

    public override void UnsubscribeToEvents()
    {
        _playerGroundCheck.OnGroundCheckChange -= OnGroundedChanged;
        _playerAttackMovement.OnCrashedDown -= OnCrashedDown;
    }

    public override void MoveSetUpdate()
    {
        base.MoveSetUpdate();
        ComboTimer();
        AttacksCooldown();
        if (_lightAttacking)
        {
            OnTryLightAttack();
        }
    }

    public override void OnLightAttack()
    {
        _lightAttacking = true;
    }

    public override void OnReleaseLightAttack()
    {
        _lightAttacking = false;
    }

    private void OnTryLightAttack()
    {
        if (_isCharging || _castTimeLeft > 0) return;

        _playerCombatSystem.SetBusyAttacking(true);

        if (_playerGroundCheck.IsGrounded())
        {
            if (_playerMovement.IsGettingMovementInput())
            {
                BreakComboIfAttackChanged(0);
                LightMoving();
                _playerMovement.SetCanMove(true);
            }
            else
            {
                BreakComboIfAttackChanged(1);
                LightInPlace();
            }
        }
        else if (!_attackedInAir)
        {
            LightInAir();
        }
    }

    private void LightMoving()
    {
        PerformLightAttack(_lightAttacksMoving[_currentCombo]);
        _currentCombo++;
        if (_currentCombo >= _lightAttacksMoving.Length)
        {
            BreakCombo();
        }
    }


    private void LightInPlace()
    {
        _playerMovement.SetCanMove(false);
        PerformLightAttack(_lightAttacksInPlace[_currentCombo]);
        _currentCombo++;
        if (_currentCombo >= _lightAttacksInPlace.Length)
        {
            BreakCombo();
        }
    }
    private void LightInAir()
    {
        _playerMovement.SetCanMove(false);
        _playerJump.StopJumpMidAir();
        _playerGravity.AddNotFallingReason("AirAttack");
        _attackedInAir = true;
        PerformLightAttack(_lightAttackInAir);
        _playerAttackMovement.SetMovement(_lightAttackInAir.Movement);
    }


    public override void OnHeavyAttack()
    {
        if (_castTimeLeft > 0 || _releasedEarly) return;

        _playerCombatSystem.SetBusyAttacking(true);

        if (_playerGroundCheck.IsGrounded())
        {
            if (_playerMovement.IsGettingMovementInput())
            {
                _currentChargedAttack = 0;
                PerformCharging(_heavyAttackMoving.ChargeableStats);
            }
            else
            {
                _currentChargedAttack = 1;
                PerformCharging(_heavyAttackInPlace.ChargeableStats);
                _playerMovement.AddSpeedModifier("AimingKick", 0);
            }
        }
        else 
        {
            _currentChargedAttack = 2;
            PerformCharging(_heavyDownAttack.ChargeableStats);
            _playerMovement.SetCanMove(false);
            _playerJump.StopJumpMidAir();
            _playerGravity.AddNotFallingReason("AirAttack");
        }

        base.OnHeavyAttack();
        BreakCombo();
    }

    public override void OnReleaseHeavyAttack()
    {
        if (_castTimeLeft > 0 || _currentCharge==0 || _releasedEarly) return; //dismiss press

        if (CheckAndHandleEarlyRelease()) return; //released too early

        switch (_currentChargedAttack)
            {
                case 0:
                    PerformHeavyAttack(_heavyAttackMoving);
                    break;
                case 1:
                    PerformHeavyAttack(_heavyAttackInPlace);
                    _playerMovement.SetCanMove(false);
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

    private void PerformLightAttack(LightAttack attack)
    {
        _playerAnimations.PlayAnimation(attack.AnimationName);
        _comboTimeLeft = _comboBreakTime + attack.CastTime;
        _castTimeLeft = attack.CastTime;
        float knockout = Random.Range(attack.Knockout.x, attack.Knockout.y);
        foreach (TriggerDamage trigger in attack.Damagers)
        {
            AddDamager(trigger);
            trigger.SetOwner(_owner);
            trigger.SetDamage(attack.Damage);
            trigger.SetKnock(attack.Knockback, knockout);
            if (_playerCombatSystem.GetAcidation)
            {
                trigger.SetAcidDamage(attack.Acid);
            }
            else
            {
                trigger.SetAcidDamage(0);
            }
        };
    }

    

    private void PerformHeavyAttack(HeavyAttack attack)
    {
        float chargePercentage = GetChargePercentage();
        float damage = Mathf.Lerp(attack.MinDamage, attack.MaxDamage, chargePercentage);
        Vector2 knockback = Vector2.Lerp(attack.MinKnockback, attack.MaxKnockback, chargePercentage);

        Vector2 knockoutChance = Vector2.Lerp(attack.MinKnockout, attack.MaxKnockout, chargePercentage);
        float knockout = Random.Range(knockoutChance.x, knockoutChance.y);

        _playerAnimations.PlayAnimation(attack.AnimationName);
        _castTimeLeft = attack.CastTime;
        foreach (TriggerDamage trigger in attack.Damagers)
        {
            AddDamager(trigger);
            trigger.SetOwner(_owner);
            trigger.SetDamage(damage);
            trigger.SetKnock(knockback, knockout);

            if (_playerCombatSystem.GetAcidation)
            {
                float Acid = Mathf.Lerp(attack.MinAcid, attack.MaxAcid, chargePercentage);
                trigger.SetAcidDamage(Acid);
            }
            else
            {
                trigger.SetAcidDamage(0);
            }

        };
        if (attack is HeavyAttackWithMovement)
        {
            HeavyAttackWithMovement attackM = (HeavyAttackWithMovement)attack;
            _playerAttackMovement.SetMovement(Vector3.Lerp(attackM.MinMovement, attackM.MaxMovement, chargePercentage));
        }
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


    private void ComboTimer()
    {
        if (_comboTimeLeft > 0)
        {
            _comboTimeLeft -= Time.deltaTime;
        }
        else if (_comboTimeLeft < 0)
        {
            BreakCombo();
        }
    }

    private void AttacksCooldown()
    {
        if (_castTimeLeft > 0)
        {
            _castTimeLeft -= Time.deltaTime;
        }
        else if (_castTimeLeft < 0)
        {
            AttackEnds();
        }
    }

    private void AttackEnds()
    {
        _castTimeLeft = 0;
        _playerMovement.SetCanMove(true);
        _playerGravity.RemoveNotFallingReason("AirAttack");
        _playerMovement.RemoveSpeedModifier("AimingKick");
        _playerGravity.ResetFall();
        _playerCombatSystem.SetBusyAttacking(false);
        RemoveAllDamagers();
    }

    public override void ResetAttacks()
    {
        base.ResetAttacks();
        _playerAnimations.PlayAnimation("Cancel");
        AttackEnds();
    }

    private void AddDamager(Damage damage)
    {
        damage.gameObject.SetActive(true);
        _currentDamagers.Add(damage);
    }

    private void RemoveAllDamagers()
    {
        foreach (Damage damage in _currentDamagers)
        {
            damage.gameObject.SetActive(false);
        }
        _currentDamagers.Clear();
    }



    private void BreakCombo()
    {
        _currentCombo = 0;
        _comboTimeLeft = 0;
    }


    private void BreakComboIfAttackChanged(int currentAttack)
    {
        if (_lastAttackType != currentAttack)
        {
            _lastAttackType = currentAttack;
            BreakCombo();
        }
    }

    private void OnGroundedChanged(bool OnGround)
    {
        if (_castTimeLeft > 0) return;

        if (OnGround)
        {
            _attackedInAir = false;
        }
        else if (_playerCombatSystem.GetCanAttack)
        {
            ResetAttacks();
            _playerMovement.SetCanMove(true);
        }
    }

    private void OnCrashedDown()
    {
        _playerAnimations.PlayAnimation(_heavyDownAttack.CrashAnimationName);
        _castTimeLeft = _heavyDownAttack.CastTime;
        _attackedInAir = false;
    }

    class BaseAttack
    {
        public string AnimationName;
        public float CastTime;
        public List<Damage> Damagers;
    }

    [System.Serializable]
    class LightAttack : BaseAttack
    {
        public float Damage, Acid;
        public Vector2 Knockback, Knockout;
    }

    [System.Serializable]
    class LightAttackWithMovement : LightAttack
    {
        public Vector3 Movement;
    }

    [System.Serializable]
    class HeavyAttack : BaseAttack
    {
        public ChargeableStats ChargeableStats;
        public float MinDamage, MaxDamage, MinAcid, MaxAcid;
        public Vector2 MinKnockback, MaxKnockback, MinKnockout, MaxKnockout;
    }

    [System.Serializable]
    class HeavyAttackWithMovement : HeavyAttack
    {
        public Vector3 MinMovement, MaxMovement;
    }

    [System.Serializable]
    class HeavyDownAttack : HeavyAttack
    {
        public float DownSpeed, MinRadius, MaxRadius, AcidationRadiusMultiplier;
        public string CrashAnimationName, AcidationVE;
    }
}
