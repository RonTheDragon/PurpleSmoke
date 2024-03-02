using UnityEngine;

public class PlayerKnockback : MonoBehaviour, IPlayerComponent
{
    private CharacterController _characterController;
    private Vector3 _knockbackVelocity;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    public void TakeKnockback(Vector2 knockback, Vector3 attackLocation)
    {
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)attackLocation).normalized;
        _knockbackVelocity = new Vector3(knockbackDirection.x * knockback.x, knockbackDirection.y * knockback.y, 0);
    }

    private void PlayerUpdate()
    {
        if (_characterController != null)
        {
            _characterController.Move(_knockbackVelocity * Time.deltaTime);

            // Gradually reduce knockback velocity over time
            _knockbackVelocity -= _knockbackVelocity * Time.deltaTime;
        }
    }
}
