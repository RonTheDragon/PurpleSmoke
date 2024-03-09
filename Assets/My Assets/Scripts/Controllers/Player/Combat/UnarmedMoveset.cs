using UnityEngine;

public class UnarmedMoveset : ChargeableMoveSet
{
    [SerializeField] private LightAttack[] _lightAttacks = new LightAttack[5];
    private PlayerAnimations _playerAnimations;
    [ReadOnly][SerializeField] private int _currentCombo;
    [SerializeField] private float _comboBreakTime;
    private float _comboTimeLeft;
    private float _castTimeLeft;
    [SerializeField] private TriggerDamage _triggerDamage;
    public override void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        base.MoveSetStart(playerCombatSystem);
        _playerAnimations = _playerCombatSystem.GetAnimations();
        _triggerDamage.SetOwner(transform.parent.gameObject);
    }

    public override void MoveSetUpdate()
    {
        base.MoveSetUpdate();
        ComboTimer();
        AttacksCooldown();
    }

    public override void OnLightAttack()
    {
        if (_isCharging || _castTimeLeft>0) return;

        PerformAttack(_lightAttacks[_currentCombo]);
        _currentCombo++;
        if (_currentCombo >= _lightAttacks.Length) 
        {
            BreakCombo(); 
        }
    }

    public override void OnReleaseLightAttack()
    {
        //left empty
    }

    public override void OnHeavyAttack()
    {
        if (_castTimeLeft > 0) return;
        base.OnHeavyAttack();
        BreakCombo();
    }

    public override void OnReleaseHeavyAttack()
    {
        if (_castTimeLeft > 0) return;
        base.OnReleaseHeavyAttack();
        BreakCombo();
    }

    private void PerformAttack(LightAttack attack)
    {
        _playerAnimations.PlayAnimation(attack.AnimationName);
        _comboTimeLeft = _comboBreakTime + attack.CastTime;
        _castTimeLeft = attack.CastTime;
        _triggerDamage.SetDamage(attack.Damage, attack.Knockback);
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
    }

    private void BreakCombo()
    {
        _currentCombo = 0;
        _comboTimeLeft = 0;
    }

    [System.Serializable]
    class LightAttack
    {
        public string AnimationName;
        public float Damage;
        public Vector2 Knockback;
        public float CastTime;
    }
}
