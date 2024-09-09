using UnityEngine;

public class PlayerKnockback : MonoBehaviour, IPlayerComponent
{
    private CharacterController _characterController;
    private Vector3 _knockbackVelocity;
    private PlayerGravity _playerGravity;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerGravity = playerComponents.GetPlayerGravity;
        playerComponents.OnUpdate += PlayerUpdate;
    }

    public void TakeKnockback(Vector2 knockback, Vector3 attackLocation)
    {
        _playerGravity.ResetFall();
        Vector3 knockbackDirection = (transform.position - attackLocation).normalized;
        _knockbackVelocity = new Vector3(knockbackDirection.x * knockback.x, knockback.y, knockbackDirection.z*knockback.x);
    }

    private void PlayerUpdate()
    {
        if (_characterController != null)
        {
            if (_knockbackVelocity.magnitude > 0.1)
            {
                _characterController.Move(_knockbackVelocity * Time.deltaTime);

                // Gradually reduce knockback velocity over time
                _knockbackVelocity -= _knockbackVelocity* 5 * Time.deltaTime;
            }
            else
            {
                _knockbackVelocity = Vector3.zero;
            }
        }
    }
}
