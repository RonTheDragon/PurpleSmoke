using UnityEngine;

public class PlayerWalk : PlayerMovement
{
    [SerializeField] private float _movementSpeed;
    public void Walk(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized * _movementSpeed * Time.deltaTime;
        _characterController.Move(new Vector3(normalizedDirection.x,0, normalizedDirection.y));
    }
}
