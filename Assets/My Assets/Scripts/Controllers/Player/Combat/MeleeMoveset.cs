using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeMoveset : ChargeableMoveSet
{
    protected PlayerGroundCheck _playerGroundCheck;
    protected PlayerWalk _playerMovement;
    protected PlayerAttackMovement _playerAttackMovement;
    protected PlayerGravity _playerGravity;
    protected PlayerJump _playerJump;
    protected bool _lightAttacking;
    protected CombatRules _owner;

    [ReadOnly][SerializeField] protected int _currentCombo;
    [SerializeField] protected float _comboBreakTime;
    protected float _comboTimeLeft;

    protected int _lastAttackType;
    protected bool _attackedInAir;
    protected int _currentChargedAttack;
    protected List<Damage> _currentDamagers = new List<Damage>();

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

    protected void OnTryLightAttack()
    {
        if (_isCharging || _castTimeLeft > 0) return;

        _playerCombatSystem.SetBusyAttacking(true);

        if (_playerGroundCheck.IsGrounded())
        {
            if (_playerMovement.IsGettingMovementInput())
            {
                BreakComboIfAttackChanged(0);
                LightMoving();
                _playerMovement.RemoveNotMovingReason("Attack");
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

    protected abstract void LightMoving();

    protected abstract void LightInPlace();

    protected abstract void LightInAir();

    public override void OnLightAttack()
    {
        _lightAttacking = true;
    }

    public override void OnReleaseLightAttack()
    {
        _lightAttacking = false;
    }


    protected void PerformLightAttack(LightAttack attack)
    {
        _playerAnimations.PlayAnimation(attack.AnimationName);
        _comboTimeLeft = _comboBreakTime + attack.CastTime;
        _castTimeLeft = attack.CastTime;
        float knockout = Random.Range(attack.Knockout.x, attack.Knockout.y);
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
        };
    }

    protected void PerformHeavyAttack(HeavyAttack attack)
    {
        float chargePercentage = GetChargePercentage();
        float damage = Mathf.Lerp(attack.MinDamage, attack.MaxDamage, chargePercentage);
        Vector2 knockback = Vector2.Lerp(attack.MinKnockback, attack.MaxKnockback, chargePercentage);

        Vector2 knockoutChance = Vector2.Lerp(attack.MinKnockout, attack.MaxKnockout, chargePercentage);
        float knockout = Random.Range(knockoutChance.x, knockoutChance.y);

        _playerAnimations.PlayAnimation(attack.AnimationName);
        _castTimeLeft = attack.CastTime;
        foreach (int i in attack.Damagers)
        {
            TriggerDamage trigger = (TriggerDamage)_playerCombatSystem.GetDamagers[i];
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

    protected void ComboTimer()
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

    protected void AttacksCooldown()
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

    protected void BreakCombo()
    {
        _currentCombo = 0;
        _comboTimeLeft = 0;
    }


    protected void BreakComboIfAttackChanged(int currentAttack)
    {
        if (_lastAttackType != currentAttack)
        {
            _lastAttackType = currentAttack;
            BreakCombo();
        }
    }

    protected void RemoveAllDamagers()
    {
        foreach (Damage damage in _currentDamagers)
        {
            damage.gameObject.SetActive(false);
        }
        _currentDamagers.Clear();
    }

    protected virtual void AttackEnds()
    {
        _castTimeLeft = 0;
        _playerMovement.RemoveNotMovingReason("Attack");
        _playerGravity.RemoveNotFallingReason("AirAttack");
        _lightAttacking = false;
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

    protected void AddDamager(Damage damage)
    {
        damage.gameObject.SetActive(true);
        _currentDamagers.Add(damage);
    }

    protected override void OnEquip()
    {
        
    }

    protected override void OnUnequip()
    {
        
    }

    protected void OnGroundedChanged(bool OnGround)
    {
        if (_castTimeLeft > 0) return;

        if (OnGround)
        {
            _attackedInAir = false;
        }
        else if (_playerCombatSystem.GetCanAttack)
        {
            ResetAttacks();
            _playerMovement.RemoveNotMovingReason("Attack");
        }
    }

    protected class MeleeAttack : AttackData
    {
        public string AnimationName;
        public float CastTime;
        public List<int> Damagers;
    }

    [System.Serializable]
    protected class LightAttack : MeleeAttack
    {
        public float Damage, Acid;
        public Vector2 Knockback, Knockout;
    }

    [System.Serializable]
    protected class LightAttackWithMovement : LightAttack
    {
        public Vector3 Movement;
    }

    [System.Serializable]
    protected class HeavyAttack : MeleeAttack
    {
        public ChargeableStats ChargeableStats;
        public float MinDamage, MaxDamage, MinAcid, MaxAcid;
        public Vector2 MinKnockback, MaxKnockback, MinKnockout, MaxKnockout;
    }

    [System.Serializable]
    protected class HeavyAttackWithMovement : HeavyAttack
    {
        public Vector3 MinMovement, MaxMovement;
    }
}
