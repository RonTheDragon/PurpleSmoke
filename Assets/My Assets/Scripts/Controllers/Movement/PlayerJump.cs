using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerJump : PlayerMovement
{
    private PlayerGravity _gravity;
    private PlayerGroundCheck _groundCheck;
    [SerializeField] private float _jumpPower = 10.0f;
    [SerializeField] private float _loseJumpPowerMult = 1f;
    
    private float _currentJumpForce;

    [SerializeField] private float _fallWhenReachVelocity = 1f;

    

    private void Update()
    {
        TryToMoveUpwards();
    }

    public override void SetPlayerController(PlayerController controller)
    {
        base.SetPlayerController(controller);
        _gravity = controller.GetPlayerGravity();
        _groundCheck = controller.GetPlayerGroundCheck();
    }

    private void TryToMoveUpwards()
    {
        if (IsJumping())
        {
            MoveUpwards();
            LoseUpwardsVelocity();
            EndJumpAndFall();
        }
    }

    private void MoveUpwards()
    {
        _characterController.Move(new Vector3(0, _currentJumpForce * Time.deltaTime, 0));
    }

    private void LoseUpwardsVelocity() 
    {
        _currentJumpForce -= _currentJumpForce* _loseJumpPowerMult*Time.deltaTime;
    }

    private void EndJumpAndFall()
    {
        if (!IsJumping())
        {
            _currentJumpForce = 0;
            _gravity.ResetFall();
            _gravity.SetCanFall(true);
        }
    }

    public void TryToInitiateJump()
    {
        if (_groundCheck.IsGrounded())
        {
            InitiateJump();
        }
    }

    private void InitiateJump() 
    {
        _gravity.SetCanFall(false);
        _currentJumpForce = _jumpPower;
    }

    

    private bool IsJumping()
    {
        if (_currentJumpForce> _fallWhenReachVelocity)
        {
            return true;
        }
        return false;
    }

    
}
