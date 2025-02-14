using System;
using System.Data;
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
    private bool _aimingBody;
    private Transform _camera;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerGravity = playerComponents.GetPlayerGravity;
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck;
        _characterBody = playerComponents.GetPlayerBody;
        _camera = playerComponents.GetCamera.transform;

        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        if (_characterController != null)
        {
            ApplyingMovement();
            ApplyingDownMovement();
            AimingBody();
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

    private void AimingBody()
    {
        if (!_aimingBody) return;

        // Get the camera's forward direction and extend it
        Vector3 lookTarget = _camera.transform.position + _camera.transform.forward * 100;

        // Make the character look at that point
        _characterBody.LookAt(lookTarget);

        // Get the rotation in Euler angles
        Vector3 eulerRotation = _characterBody.localRotation.eulerAngles;

        // If aiming upwards (X rotation is greater than 180), lock it to 0
        if (eulerRotation.x > 180)
            eulerRotation.x = 0;

        // Apply corrected rotation
        _characterBody.localRotation = Quaternion.Euler(eulerRotation);
    }








    public void SetAimingBody(bool b)
    {
        _aimingBody = b;
        if (!_aimingBody)
        {
            Vector3 eulerRotation = _characterBody.localRotation.eulerAngles;
            _characterBody.localRotation = Quaternion.Euler(0, eulerRotation.y, 0);
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
