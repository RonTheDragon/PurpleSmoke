using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour,IPlayerComponent
{
    [ReadOnly][SerializeField] private float _currentSpeed;
    [SerializeField] private float _baseWalkingSpeed;
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
    private bool _canMove = true;
    private Vector2 _movementInput;
    private bool _lockOnForward;

    private Dictionary<string, float> _speedModifiers = new Dictionary<string, float>();

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerBody = playerComponents.GetPlayerBody;
        _camera = playerComponents.GetCamera;
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _groundCheck = playerComponents.GetPlayerGroundCheck;
        _groundCheck.OnGroundCheckChange += ChangeSpeedToAir;
        _currentSpeed = _baseWalkingSpeed;
        SetAnimationDirectionForward();
    }

    public void Walk(Vector2 direction)
    {
        _movementInput = direction;
        if (!_canMove) { return; }


        if (_lockOnForward)
        {
            RotatePlayerForward();
            _playerAnimations.SetWalkDirection(direction);
        }

        if (IsGettingMovementInput())
        {
            _normalizedDirection = _movementInput.normalized;
            _targetAngle = CalculateTargetAngle();
            if (!_lockOnForward)
            {
                RotatePlayer();
            }
            MovePlayer();
            _playerAnimations.ChangeWalk(1);
        }
        else
        {
            _playerAnimations.ChangeWalk(0);
        }
    }

    public bool IsGettingMovementInput()
    {
        return _movementInput.magnitude > 0;
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

    private void RotatePlayerForward()
    {
        _angle = Mathf.SmoothDampAngle(_playerBody.eulerAngles.y, _camera.transform.eulerAngles.y, ref _currentTurnVelocity, _currentTurnSpeed);
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
            RemoveSpeedModifier("InAir");
        }
        else
        {
            AddSpeedModifier("InAir", _airMovementSpeed);
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    public void AddSpeedModifier(string modifierName, float modifierValue)
    {
        if (_speedModifiers.ContainsKey(modifierName))
        {
            return; //already exist
        }
        else
        {
            _speedModifiers.Add(modifierName, modifierValue);
        }

        UpdateCurrentSpeed();
    }

    public void RemoveSpeedModifier(string modifierName)
    {
        if (_speedModifiers.ContainsKey(modifierName))
        {
            _speedModifiers.Remove(modifierName);
            UpdateCurrentSpeed();
        }
    }

    private void UpdateCurrentSpeed()
    {
        _currentSpeed = _baseWalkingSpeed;
        float walkSpeedForAnimation = 1;
        foreach (float modifierValue in _speedModifiers.Values)
        {
            _currentSpeed *= modifierValue;
            walkSpeedForAnimation *= modifierValue;
        }
        _playerAnimations.SetWalkSpeed(walkSpeedForAnimation);
    }

    public void SetLockOnForward(bool lockOnForward)
    {
        _lockOnForward = lockOnForward;
        if (!_lockOnForward ) { SetAnimationDirectionForward(); }
    }

    private void SetAnimationDirectionForward()
    {
        _playerAnimations.SetWalkDirection(new Vector2(0, 1));
    }

}
