
public class PlayerKnockout : CharacterKnockout, IPlayerComponent
{
    private PlayerAnimations _playerAnimations;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerCharging _playerCharging;
    private PlayerWalk _playerWalk;
    private PlayerJump _playerJump;
    private PlayerHealth _playerHealth;
    private PlayerGravity _playerGravity;
    private PlayerAimMode _playerAimMode;


    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _playerWalk = playerComponents.GetPlayerWalk;
        _playerJump = playerComponents.GetPlayerJump;
        _playerGravity = playerComponents.GetPlayerGravity;
        _playerAimMode = playerComponents.GetPlayerAimMode;
        _playerHealth = playerComponents.GetPlayerHealth;
        _playerCharging = playerComponents.GetPlayerCharging;
        playerComponents.OnUpdate += PlayerUpdate;

        // Check if caps are set correctly
        CheckIfCapsCorrect();
    }

    private void PlayerUpdate()
    {
        if (_playerHealth.GetIsDead) return;

        StunCheckLoop();
    }

    protected override void SetAnimationWeight(float weight)
    {
        _playerAnimations.SetLayerWeight(2, weight);
    }

    protected override void PlayAnimation(string animationName)
    {
        _playerAnimations.PlayAnimation(animationName);
    }

    protected override void ClearAttacks()
    {
        _playerCombatSystem.ClearAttacks();
       // _playerCharging.ForceResetCharge();
    }

    public override void StunCharacter()
    {
        base.StunCharacter();

        _playerWalk.AddNotMovingReason("Stun");
        _playerJump.SetCanJump(false);
        _playerCombatSystem.SetCanAttack(false);
        _playerGravity.ClearNotFallingReasons();
        _playerAimMode.SetLockHeadAim(true);
    }

    public override void UnStunCharacter()
    {
        if (_playerHealth.GetIsDead) return;

        base.UnStunCharacter();

        _playerWalk.RemoveNotMovingReason("Stun");
        _playerJump.SetCanJump(true);
        _playerCombatSystem.SetCanAttack(true);
        _playerAimMode.SetLockHeadAim(false);
    }

    
}
