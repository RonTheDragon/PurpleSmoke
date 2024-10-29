using UnityEngine;

public class PlayerKnockback : CharacterKnockback, IPlayerComponent
{
    private PlayerGravity _playerGravity;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerGravity = playerComponents.GetPlayerGravity;
        playerComponents.OnUpdate += PlayerUpdate;
    }

    public override void TakeKnockback(Vector2 knockback, Vector3 attackLocation)
    {
        _playerGravity.ResetFall();
        base.TakeKnockback(knockback, attackLocation);
    }

    

    private void PlayerUpdate()
    {
        KnockbackLoop();
    }
}
