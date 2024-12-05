

using UnityEngine;

public class FangsMoveset : MeleeMoveset
{
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
        
    }

    protected override void LightInPlace()
    {
        
    }

    protected override void LightMoving()
    {
        
    }

    protected override void OnEquip()
    {
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
        _rightFang.gameObject.SetActive(false);
        _leftFang.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(_rightFang.gameObject);
        Destroy(_leftFang.gameObject);
    }
}
