using System;
using UnityEngine;

public class PlayerAttackMovement : MonoBehaviour, IPlayerComponent
{
    private Transform _playerBody;
    private CharacterController _characterController;
    private PlayerGravity _playerGravity;
    private PlayerGroundCheck _playerGroundCheck;
    private Vector3 _currentAttackMovement;
    [SerializeField] private Vector3 _incomingAttackMovement;
    private bool _crashingDown;
    private float _downMovementSpeed;
    public Action OnCrashedDown;
    private bool _fallingCheckDelayed;
    private float _fallingCheckDelayTimer;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController();
        _playerGravity = playerComponents.GetPlayerGravity();
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck();
        _playerBody = playerComponents.GetPlayerBody();
    }

    public void SetAndApplyMovement(Vector3 movement)
    {
        SetMovement(movement);
        ApplyMovement();
    }

    public void SetMovement(Vector3 movement)
    {
        _incomingAttackMovement = movement;
    }

    public void ApplyMovement()
    {
        _currentAttackMovement = _incomingAttackMovement;
    }

    private void Update()
    {
        if (_characterController != null)
        {
            ApplyingMovement();
            ApplyingDownMovement();
        }
    }

    private void ApplyingMovement()
    {
        if (_currentAttackMovement.magnitude > 0.1f)
        {
            _characterController.Move(((_playerBody.forward* _currentAttackMovement.z) 
                + (Vector3.up * _currentAttackMovement.y) + (_playerBody.right * 
                _currentAttackMovement.x)) * Time.deltaTime);
            // Gradually reduce attack velocity over time
            _currentAttackMovement -= _currentAttackMovement * 5 * Time.deltaTime;
        }
        else
        {
            _currentAttackMovement = Vector3.zero;
        }
    }

    private void ApplyingDownMovement()
    {
        if (_crashingDown)
        {
            _characterController.Move(Vector3.down * _downMovementSpeed * Time.deltaTime);
            if (StuckCheck() || _playerGroundCheck.IsGrounded())
            {
                _crashingDown = false;
                OnCrashedDown?.Invoke();
            }
        }
    }

    private bool StuckCheck()
    {
        if (!_fallingCheckDelayed)
        {
            // Start the delay timer
            _fallingCheckDelayTimer += Time.deltaTime;
            if (_fallingCheckDelayTimer >= 0.5f)
            {
                _fallingCheckDelayed = true;
                _fallingCheckDelayTimer = 0f; // Reset the delay timer
            }
            return false;
        }
        else
        {
            // Check if the player is actually falling after the delay
            if (_playerGravity.IsActuallyFalling())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }



    public void SetCrashingDownSpeed(float speed)
    {
        _downMovementSpeed = speed;
    }

    public void CrashDown()
    {
        _crashingDown = true;
        _fallingCheckDelayed = false;
        _fallingCheckDelayTimer = 0f;
    }

    public bool IsStillCrashingDown()
    {
        return _crashingDown;
    }
}
