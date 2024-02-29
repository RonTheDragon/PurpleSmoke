using UnityEngine;

public class PlayerWalk : MonoBehaviour,IPlayerComponent
{
    [ReadOnly][SerializeField] private float _currentSpeed;
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _airMovementSpeed;
    [SerializeField] private float _currentTurnSpeed = 0.1f;

    private CharacterController _characterController;
    private Transform _playerBody;
    private Camera _camera;
    private float _currentTurnVelocity;
    private PlayerGroundCheck _groundCheck;
    private PlayerAnimations _playerAnimations;

    private Vector2 _normalizedDirection;
    private float _targetAngle;
    private float _angle;
    private Vector3 _moveDirection;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController();
        _playerBody = playerComponents.GetPlayerBody();
        _camera = playerComponents.GetCamera();
        _playerAnimations = playerComponents.GetPlayerAnimations();
        _groundCheck = playerComponents.GetPlayerGroundCheck();
        _groundCheck.OnGroundCheckChange += ChangeSpeedToAir;
        _currentSpeed = _walkingSpeed;
    }

    public void Walk(Vector2 direction)
    {
        if (direction.magnitude > 0)
        {
            _normalizedDirection = direction.normalized;
            _targetAngle = CalculateTargetAngle();
            RotatePlayer();
            MovePlayer();
            _playerAnimations.ChangeWalk(1);
        }
        else
        {
            _playerAnimations.ChangeWalk(0);
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
        _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);
    }

    private void ChangeSpeedToAir(bool ground)
    {
        if (ground)
        {
            _currentSpeed = _walkingSpeed;
        }
        else
        {
            _currentSpeed = _airMovementSpeed;
        }
    }

}
