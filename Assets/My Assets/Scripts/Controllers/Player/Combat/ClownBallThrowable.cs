using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownBallThrowable : UseableAbility
{
    [SerializeField] private string ChargeThrowAnim, ThrowAnim;
    private PlayerAnimations _playerAnimations;
    public override void UseableStart(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
        PlayerComponentsRefrences refs = playerCombatSystem.GetPlayerRefs;
        _playerAnimations = refs.GetPlayerAnimations;
    }

    public override void OnPress()
    {
        if (!_playerCombatSystem.GetIsBusyAttacking)
        {
            _playerAnimations.PlayAnimation(ChargeThrowAnim);
            
        }
    }

    public override void OnRelease()
    {
        
    }
}
