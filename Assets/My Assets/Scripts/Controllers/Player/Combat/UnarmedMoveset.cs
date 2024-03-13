using UnityEngine;

public class UnarmedMoveset : ChargeableMoveSet
{
    [SerializeField] private LightAttack[] _lightAttacksMoving = new LightAttack[5];
    [SerializeField] private LightAttack[] _lightAttacksInPlace = new LightAttack[3];
    [SerializeField] private LightAttackWithMovement _lightAttackInAir;
    [SerializeField] private HeavyAttack _heavyAttackMoving;
    [SerializeField] private HeavyAttackWithMovement _heavyAttackInPlace;
    [SerializeField] private HeavyDownAttack _heavyDownAttack;

    private PlayerAnimations _playerAnimations;
    private PlayerGroundCheck _playerGroundCheck;
    private PlayerWalk _playerMovement;
    private PlayerAttackMovement _playerAttackMovement;
    private PlayerGravity _playerGravity;
    [ReadOnly][SerializeField] private int _currentCombo;
    [SerializeField] private float _comboBreakTime;
    private float _comboTimeLeft;
    private float _castTimeLeft;
    [SerializeField] private TriggerDamage _triggerDamage;
    [SerializeField] private ExplosionDamage _explosionDamage;
    private int _lastAttackType;
    private bool _attackedInAir;
    private int _currentChargedAttack;
    public override void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        base.MoveSetStart(playerCombatSystem);
        _playerAnimations = _playerCombatSystem.GetAnimations();
        _playerGroundCheck = _playerCombatSystem.GetGroundCheck();
        _playerGroundCheck.OnGroundCheckChange += OnGroundedChanged;
        _playerMovement = _playerCombatSystem.GetMovement();
        _playerAttackMovement = _playerCombatSystem.GetAttackMovement();
        _playerGravity = _playerCombatSystem.GetGravity();
        _playerAttackMovement.OnCrashedDown += OnCrashedDown;
        _triggerDamage.SetOwner(transform.parent.gameObject);
        _explosionDamage.SetOwner(transform.parent.gameObject);
    }

    public override void MoveSetUpdate()
    {
        base.MoveSetUpdate();
        ComboTimer();
        AttacksCooldown();
    }

    public override void OnLightAttack()
    {
        if (_isCharging || _castTimeLeft > 0) return;

        if (_playerGroundCheck.IsGrounded())
        {
            if (_playerMovement.IsGettingMovementInput())
            {
                BreakComboIfAttackChanged(0);
                LightMoving();
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
        _playerGravity.SetCanFall(false);
        _attackedInAir = true;
        PerformLightAttack(_lightAttackInAir);
        _playerAttackMovement.SetMovement(_lightAttackInAir.Movement);
    }

    public override void OnReleaseLightAttack()
    {
        //left empty
    }

    public override void OnHeavyAttack()
    {
        if (_castTimeLeft > 0) return;

        if (_playerGroundCheck.IsGrounded())
        {
            if (_playerMovement.IsGettingMovementInput())
            {
                _currentChargedAttack = 0;
                PerformCharging(_heavyAttackMoving);
            }
            else
            {
                _currentChargedAttack = 1;
                PerformCharging(_heavyAttackInPlace);
                _playerMovement.SetCanMove(false);
            }
        }
        else 
        {
            _currentChargedAttack = 2;
            _playerMovement.SetCanMove(false);
            _playerGravity.SetCanFall(false);
            _playerGravity.ResetFall();
            PerformCharging(_heavyDownAttack);
        }

        base.OnHeavyAttack();
        BreakCombo();
    }

    public override void OnReleaseHeavyAttack()
    {
        if (_castTimeLeft > 0) return;

        switch (_currentChargedAttack)
        {
            case 0:
                PerformHeavyAttack(_heavyAttackMoving);
                break;
            case 1:
                PerformHeavyAttack(_heavyAttackInPlace);
                _playerAttackMovement.SetMovement(_heavyAttackInPlace.Movement);
                break;
            case 2:
                PerformHeavyAttackExplosive(_heavyDownAttack);
                _playerAttackMovement.SetCrashingDownSpeed(_heavyDownAttack.DownSpeed);
                _castTimeLeft = 100;
                break;
            default:
                break;
        }

        base.OnReleaseHeavyAttack();
        BreakCombo();
    }

    private void PerformLightAttack(LightAttack attack)
    {
        _playerAnimations.PlayAnimation(attack.AnimationName);
        _comboTimeLeft = _comboBreakTime + attack.CastTime;
        _castTimeLeft = attack.CastTime;
        _triggerDamage.SetDamage(attack.Damage, attack.Knockback);
    }

    private void PerformCharging(HeavyAttack attack)
    {
        _maxCharge = attack.ChargeTime;
        _playerAnimations.PlayAnimation(attack.ChargeAnimationName);
    }

    private void PerformHeavyAttack(HeavyAttack attack)
    {
        float chargePercentage = GetChargePercentage();
        float damage = Mathf.Lerp(attack.MinDamage, attack.MaxDamage, chargePercentage);
        Vector2 knockback = Vector2.Lerp(attack.MinKnockback, attack.MaxKnockback, chargePercentage);

        _playerAnimations.PlayAnimation(attack.AnimationName);
        _castTimeLeft = attack.CastTime;
        _explosionDamage.SetDamage(damage, knockback);
    }

    private void PerformHeavyAttackExplosive(HeavyDownAttack attack)
    {
        PerformHeavyAttack(attack);

        float chargePercentage = GetChargePercentage();
        float radius = Mathf.Lerp(attack.MinRadius, attack.MaxRadius, chargePercentage);
        _explosionDamage.SetRadius(radius);
    }

    public void PerformExplosionDamage()
    {
        _explosionDamage.Explode(); 
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
            _castTimeLeft = 0;
            _playerMovement.SetCanMove(true);
            _playerGravity.SetCanFall(true);
            _playerGravity.ResetFall();
        }
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
        if (OnGround)
        {
            _attackedInAir = false;
        }
        else
        {
            ResetCharge();
            _playerMovement.SetCanMove(true);
        }
    }

    private void OnCrashedDown()
    {
        _playerAnimations.PlayAnimation(_heavyDownAttack.CrashAnimationName);
        _castTimeLeft = _heavyDownAttack.CastTime;
    }

    [System.Serializable]
    class LightAttack
    {
        public string AnimationName;
        public float Damage;
        public Vector2 Knockback;
        public float CastTime;
    }

    [System.Serializable]
    class LightAttackWithMovement : LightAttack
    {
        public Vector3 Movement;
    }

    [System.Serializable]
    class HeavyAttack
    {
        public string ChargeAnimationName;
        public float ChargeTime;
        public string AnimationName;
        public float MinDamage;
        public float MaxDamage;
        public Vector2 MinKnockback;
        public Vector2 MaxKnockback;
        public float CastTime;
        public bool ReleaseOnFull;
    }

    [System.Serializable]
    class HeavyAttackWithMovement : HeavyAttack
    {
        public Vector3 Movement;
    }

    [System.Serializable]
    class HeavyDownAttack : HeavyAttack
    {
        public float DownSpeed;
        public string CrashAnimationName;
        public float MinRadius;
        public float MaxRadius;
    }
}
