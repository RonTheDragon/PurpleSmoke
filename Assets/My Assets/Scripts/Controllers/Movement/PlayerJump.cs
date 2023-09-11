using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerJump : PlayerMovement
{
    private PlayerGravity _gravity;
    private PlayerGroundCheck _groundCheck;
    private Camera _camera;
    [SerializeField] private float _jumpPower = 10.0f;
    [SerializeField] private float _loseJumpPowerMult = 1f;
    [SerializeField] private float _jumpMovementPower = 5;

    private float _currentJumpForce;

    [SerializeField] private float _fallWhenReachVelocity = 1f;

    [ReadOnly][SerializeField] private bool _doubleJump;

    private Vector2 _jumpMovementInput;

    [ReadOnly][SerializeField]private Vector2 _currentJumpMovement;

    

    private void Update()
    {
        TryToMoveUpwards();
    }

    public override void SetPlayerController(PlayerController controller)
    {
        base.SetPlayerController(controller);
        _gravity = controller.GetPlayerGravity();
        _camera = controller.GetCamera();
        _groundCheck = controller.GetPlayerGroundCheck();
        _groundCheck.OnGroundCheckChange += ResetDoubleJump;
    }

    private void TryToMoveUpwards()
    {
        if (IsJumping())
        {
            MoveUpwards();
            LoseUpwardsVelocity();
            MoveSideways();
            LoseSidewaysVelocity();
            EndJumpAndFall();
        }
    }

    private void MoveUpwards()
    {
        _characterController.Move(new Vector3(0, _currentJumpForce * Time.deltaTime, 0));
    }

    private void MoveSideways()
    {
        _characterController.Move(new Vector3(_currentJumpMovement.x,0, _currentJumpMovement.y)*Time.deltaTime);
    }

    private void LoseUpwardsVelocity() 
    {
        _currentJumpForce -= _currentJumpForce* _loseJumpPowerMult*Time.deltaTime;
    }

    private void LoseSidewaysVelocity()
    {
        _currentJumpMovement -= _currentJumpMovement* _loseJumpPowerMult * Time.deltaTime;
    }

    private void EndJumpAndFall()
    {
        if (!IsJumping())
        {
            _currentJumpForce = 0;
            _currentJumpMovement = Vector2.zero;
            _gravity.ResetFall();
            _gravity.SetCanFall(true);
        }
    }

    public void TryToInitiateJump(Vector2 movement)
    {
        _jumpMovementInput = movement;
        if (_groundCheck.IsGrounded())
        {
            InitiateJump();
        }
        else if (_doubleJump)
        {
            InitiateJump();
            _doubleJump= false;
        }
    }

    private void InitiateJump() 
    {
        _playerController.StopCharacterController();
        _gravity.SetCanFall(false);
        _currentJumpForce = _jumpPower;
        HandleJumpMovement();
    }

    private void HandleJumpMovement()
    {
        if (_jumpMovementInput.magnitude > 0)
        {
            Vector2 normalized = _jumpMovementInput.normalized;
            Vector3 v3 = Quaternion.Euler(0, CalculateTargetAngle(normalized), 0)
                * Vector3.forward * _jumpMovementPower;
            _currentJumpMovement= new Vector2(v3.x, v3.z);
        }
    }

    private float CalculateTargetAngle(Vector2 _normalizedDirection)
    {
        return Mathf.Atan2(_normalizedDirection.x, _normalizedDirection.y)
            * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
    }


    private bool IsJumping()
    {
        if (_currentJumpForce> _fallWhenReachVelocity)
        {
            return true;
        }
        return false;
    }

    private void ResetDoubleJump(bool ground)
    {
        if (ground)
        {
            _doubleJump = true;
        }
    }

    public bool CanGlide()
    {
        return !_doubleJump;
    }
    
}
