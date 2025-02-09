

using UnityEngine;

public class FangsMoveset : MeleeMoveset
{
    [SerializeField] private LightAttack[] _lightAttacksMoving = new LightAttack[2];
    [SerializeField] private LightAttackWithMovement[] _lightAttacksInPlace = new LightAttackWithMovement[2];
    [SerializeField] private LightAttack _lightAttackInAir;
    [SerializeField] private Transform _rightFang, _leftFang;
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
    }

    protected override void OnUnequip()
    {
        _playerGroundCheck.OnGroundCheckChange -= OnGroundedChanged;
        _rightFang.gameObject.SetActive(false);
        _leftFang.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(_rightFang.gameObject);
        Destroy(_leftFang.gameObject);
    }

    //Unfinished Section
    protected override void HeavyMoving()
    {
       
    }

    protected override void HeavyInPlace()
    {
        
    }

    protected override void HeavyInAir()
    {
        
    }

    public override void OnReleaseHeavyAttack()
    {
        base.OnReleaseHeavyAttack();
        ResetAttacks();
    }
}
