using System;
using UnityEngine;

public class PlayerAttackMovement : CharacterAttackMovement, IPlayerComponent
{
    private PlayerGravity _playerGravity;
    private PlayerGroundCheck _playerGroundCheck;
    private bool _crashingDown;
    [SerializeField] private float _downMovementSpeed;
    public Action OnCrashedDown;
    private bool _fallingCheckDelayed;
    private float _fallingCheckDelayTimer;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerGravity = playerComponents.GetPlayerGravity;
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck;
        _characterBody = playerComponents.GetPlayerBody;

        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        if (_characterController != null)
        {
            ApplyingMovement();
            ApplyingDownMovement();
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
            _fallingCheckDelayTimer += Time.deltaTime;
            if (_fallingCheckDelayTimer >= 0.5f)
            {
                _fallingCheckDelayed = true;
                _fallingCheckDelayTimer = 0f;
            }
            return false;
        }
        else
        {
            return _playerGravity.IsActuallyFalling();
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
