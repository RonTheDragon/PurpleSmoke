using UnityEngine;

public class PlayerWalk : PlayerMovement
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _currentTurnSpeed = 0.1f;

    private Transform _playerBody;
    private Camera _camera;
    private float _currentTurnVelocity;

    private Vector2 _normalizedDirection;
    private float _targetAngle;
    private float _angle;
    private Vector3 _moveDirection;

    public override void SetPlayerController(PlayerController controller)
    {
        base.SetPlayerController(controller);
        _playerBody = controller.GetPlayerBody();
        _camera = controller.GetCamera();
    }

    public void Walk(Vector2 direction)
    {
        if (direction.magnitude > 0)
        {
            _normalizedDirection = direction.normalized;
            _targetAngle = CalculateTargetAngle();
            RotatePlayer();
            MovePlayer();
        }
    }

    private float CalculateTargetAngle()
    {
        return Mathf.Atan2(_normalizedDirection.x, _normalizedDirection.y)
            * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
    }

    private void RotatePlayer()
    {
        _angle = Mathf.SmoothDampAngle(_playerBody.eulerAngles.y, _targetAngle, ref _currentTurnVelocity, _currentTurnSpeed);
        _playerBody.rotation = Quaternion.Euler(0, _angle, 0);
    }

    private void MovePlayer()
    {
        _moveDirection = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        _characterController.Move(_moveDirection * _movementSpeed * Time.deltaTime);
    }
}
