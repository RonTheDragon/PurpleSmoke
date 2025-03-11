using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FangsMoveset : MeleeMoveset
{
    [SerializeField] private LightAttack[] _lightAttacksMoving = new LightAttack[2];
    [SerializeField] private LightAttackWithMovement[] _lightAttacksInPlace = new LightAttackWithMovement[2];
    [SerializeField] private LightAttack _lightAttackInAir;
    [SerializeField] private HeavyRepeatingAttack _heavyAttackMoving;
    [SerializeField] private HeavyAttackWithMovement _heavyAttackInPlace;
    [SerializeField] private HeavyAttackWithMovement _heavyAirAttack;
    [SerializeField] private Transform _rightFang, _leftFang;
    [SerializeField] private TrailRenderer _rightTrail, _leftTrail;
    private PlayerAcidation _playerAcidation;
    public override void MoveSetStart(CombatSystem combatSystem)
    {
        base.MoveSetStart(combatSystem);
        _playerCombatSystem = (PlayerCombatSystem)combatSystem;
        PlayerComponentsRefrences refs = _playerCombatSystem.GetPlayerRefs;
        _playerAnimations = refs.GetPlayerAnimations;
        _playerGroundCheck = refs.GetPlayerGroundCheck;
        _playerMovement = refs.GetPlayerWalk;
        _playerAttackMovement = refs.GetPlayerAttackMovement;
        _playerGravity = refs.GetPlayerGravity;
        _playerJump = refs.GetPlayerJump;
        _owner = refs.GetCombatRules;
        _playerAcidation = refs.GetPlayerAcidation;
        _playerCharging = refs.GetPlayerCharging;
    }

    protected override void LightInAir()
    {
        _playerMovement.AddNotMovingReason("Attack");
        _playerJump.StopJumpMidAir();
        _playerGravity.AddNotFallingReason("AirAttack");
        _attackedInAir = true;
        PerformLightAttack(_lightAttackInAir);
        _playerCombatSystem.SpendMelee();
    }

    protected override void LightInPlace()
    {
        _playerMovement.AddNotMovingReason("Attack");
        PerformLightAttack(_lightAttacksInPlace[_currentCombo]);
        _playerAttackMovement.SetMovement(_lightAttacksInPlace[_currentCombo].Movement);
        _currentCombo++;
        if (_currentCombo >= _lightAttacksInPlace.Length)
        {
            BreakCombo();
        }
        _playerCombatSystem.SpendMelee();
    }

    protected override void LightMoving()
    {
        if (_playerCombatSystem.IsMeleeEmpty()) return;
        PerformLightAttack(_lightAttacksMoving[_currentCombo]);
        _currentCombo++;
        if (_currentCombo >= _lightAttacksMoving.Length)
        {
            BreakCombo();
        }
        _playerCombatSystem.SpendMelee();
    }

    protected override void AttackEnds()
    {
        base.AttackEnds();
        _playerCombatSystem.DeleteIfEmptyMelee();
        StopAllCoroutines();
    }

    protected override void OnEquip()
    {
        _playerGroundCheck.OnGroundCheckChange += OnGroundedChanged;
        _rightFang.parent = _playerCombatSystem.GetRightHand;
        _leftFang.parent = _playerCombatSystem.GetLeftHand;
        _rightFang.localPosition = Vector3.zero;
        _leftFang.localPosition = Vector3.zero;
        _rightFang.localRotation = Quaternion.identity;
        _leftFang.localRotation = Quaternion.identity;
        _rightFang.localScale = Vector3.one;
        _leftFang.localScale = Vector3.one;
        _rightFang.gameObject.SetActive(true);
        _leftFang.gameObject.SetActive(true);
        _playerAcidation.AddToActiveWhileAcidation(_rightTrail.gameObject);
        _playerAcidation.AddToActiveWhileAcidation(_leftTrail.gameObject);
        _playerAnimations.SetTrails(new List<TrailRenderer> { _rightTrail, _leftTrail });
    }

    protected override void OnUnequip()
    {
        _playerGroundCheck.OnGroundCheckChange -= OnGroundedChanged;
        _rightFang.gameObject.SetActive(false);
        _leftFang.gameObject.SetActive(false);
        _playerAcidation.RemoveFromActiveWhileAcidation(_rightTrail.gameObject);
        _playerAcidation.RemoveFromActiveWhileAcidation(_leftTrail.gameObject);
        _playerAnimations.ClearTrails();
    }

    private void OnDestroy()
    {
       // _playerAcidation.RemoveFromActiveWhileAcidation(_rightTrail.gameObject);
       // _playerAcidation.RemoveFromActiveWhileAcidation(_leftTrail.gameObject);
      //  _playerAnimations.ClearTrails();
        Destroy(_rightFang.gameObject);
        Destroy(_leftFang.gameObject);
    }

    //Unfinished Section
    protected override void HeavyMoving()
    {
        _playerCharging.PerformCharging(_heavyAttackMoving.ChargeableStats, OnReleaseHeavyAttack);
    }

    protected override void HeavyInPlace()
    {
        _playerCharging.PerformCharging(_heavyAttackInPlace.ChargeableStats, OnReleaseHeavyAttack);
    }

    protected override void HeavyInAir()
    {
        _playerCharging.PerformCharging(_heavyAirAttack.ChargeableStats, OnReleaseHeavyAttack);
        _playerAttackMovement.SetAimingBody(true);
    }

    public override void OnReleaseHeavyAttack()
    {
        base.OnReleaseHeavyAttack();
        if (_castTimeLeft > 0 || _playerCharging.GetChargePercentage() == 0 || _playerCharging.GetReleasedEarly) return; //dismiss press

        if (_playerCharging.CheckAndHandleEarlyRelease()) return; //released too early

        switch (_currentChargedAttack)
        {
            case 0:
                PerformHeavyAttackRepeating(_heavyAttackMoving);
                break;
            case 1:
                PerformHeavyAttack(_heavyAttackInPlace);
                _playerMovement.AddNotMovingReason("Attack");
                _playerMovement.RemoveSpeedModifier("AimingKick");
                _playerCombatSystem.SpendMelee(2);
                break;
            case 2:
                PerformChopperAttack(_heavyAirAttack);
                _playerCombatSystem.SpendMelee(2);
                break;
            default:
                break;
        }
        _playerCharging.ResetCharge();
        BreakCombo();
    }

    private void PerformHeavyAttackRepeating(HeavyRepeatingAttack attack)
    {
        float chargePercentage = _playerCharging.GetChargePercentage();
        int repeats = Mathf.Max(1, (int)(attack.MaxRepeat * chargePercentage)); // Ensure at least one repeat
        float knockout = Random.Range(attack.Knockout.x, attack.Knockout.y);

        StartCoroutine(RepeatAttackAnimation(attack.AnimationName, repeats, attack.CastTime)); // Add delay per repeat

        _castTimeLeft = attack.CastTime*repeats;

        foreach (int i in attack.Damagers)
        {
            TriggerDamage trigger = (TriggerDamage)_playerCombatSystem.GetDamagers[i];
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
        }
    }

    private IEnumerator RepeatAttackAnimation(string animationName, int repeats, float delay)
    {
        for (int i = 0; i < repeats; i++)
        {
            _playerAnimations.PlayAnimation(animationName);
            _playerCombatSystem.SpendMelee();
            yield return new WaitForSeconds(delay); // Wait before playing next repeat
        }
    }

    private void PerformChopperAttack(HeavyAttackWithMovement attack)
    {
        PerformHeavyAttack(attack);
        _playerAttackMovement.ApplyMovement();
        Invoke(nameof(EndChopperAttack), attack.CastTime);
    }

    private void EndChopperAttack()
    {
        _playerAttackMovement.SetAimingBody(false);
        _playerAnimations.PlayAnimation("Cancel");
        _playerMovement.RemoveNotMovingReason("Attack");
        _playerGravity.RemoveNotFallingReason("AirAttack");
    }

    public override void ResetAttacks()
    {
        base.ResetAttacks();
        StopAllCoroutines();
    }

    [System.Serializable]
    protected class HeavyRepeatingAttack : LightAttack
    {
        public ChargeableStats ChargeableStats;
        public int MaxRepeat;
    }
}
