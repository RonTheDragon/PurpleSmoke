using System;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour, IPlayerComponent
{
    private PlayerGroundCheck _playerGroundCheck;
    private int _currentWalk;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _walkInt;
    [SerializeField] private string _walkSpeedFloat;
    [SerializeField] private string _walkXFloat;
    [SerializeField] private string _walkYFloat;
    [SerializeField] private float _walkDirectionLerpSpeed;
    [SerializeField] private string _jumpTrigger;
    [SerializeField] private string _flipTrigger;
    [SerializeField] private string _groundedBool;
    [SerializeField] private string _glideBool;

    private Vector2 _currentWalkDirection;
    private Vector2 _WalkDirectionTarget;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck();
        _playerGroundCheck.OnGroundCheckChange += ChangeGrounded;
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        ApplyWalkDirection();
    }

    private void ChangeGrounded(bool grounded)
    {
        _animator.SetBool(_groundedBool,grounded);
    }

    public void ChangeWalk(int walkInt)
    {
        if (_currentWalk != walkInt)
        {
            _currentWalk = walkInt;
            _animator.SetInteger(_walkInt, walkInt);
        }
    }

    public void SetWalkSpeed(float speed)
    {
        _animator.SetFloat(_walkSpeedFloat, speed);
    }

    public void SetWalkDirection(Vector2 direction)
    {
        _WalkDirectionTarget = direction;
    }

    private void ApplyWalkDirection()
    {
        if (_currentWalkDirection != _WalkDirectionTarget)
        {
            _currentWalkDirection = Vector2.Lerp(_currentWalkDirection, _WalkDirectionTarget, Time.deltaTime * _walkDirectionLerpSpeed);

            _animator.SetFloat(_walkXFloat, _currentWalkDirection.x);
            _animator.SetFloat(_walkYFloat, _currentWalkDirection.y);
        }
    }

    public void ChangeGlide(bool glide)
    {
        _animator.SetBool(_glideBool, glide);
    }

    public void Jump()
    {
        _animator.SetTrigger(_jumpTrigger);
        _animator.SetBool(_groundedBool, false);
    }

    public void Flip() 
    {
        _animator.SetTrigger(_flipTrigger);
    }

    public void PlayAnimation(string animationName)
    {
        if (animationName == string.Empty) return;

        _animator.SetTrigger(animationName);
    }

    public void SetLayerWeight(int layer,float amount)
    {
        _animator.SetLayerWeight(layer, amount);
    }
}
