using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimations : CharacterAnimations, IPlayerComponent
{
    private PlayerGroundCheck _playerGroundCheck;
    
    [SerializeField] private string _walkSpeedFloat;
    [SerializeField] private string _walkXFloat;
    [SerializeField] private string _walkYFloat;
    [SerializeField] private float _walkDirectionLerpSpeed;
    [SerializeField] private string _jumpTrigger;
    [SerializeField] private string _flipTrigger;
    [SerializeField] private string _groundedBool;
    [SerializeField] private string _glideBool;
    [SerializeField] private MultiAimConstraint _headAim;

    private float _headAimWeight;
    [SerializeField] private float _headAimWeightLerpSpeed=5;

    private Vector2 _currentWalkDirection;
    private Vector2 _WalkDirectionTarget;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck;
        _playerGroundCheck.OnGroundCheckChange += ChangeGrounded;
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void PlayerUpdate()
    {
        ApplyWalkDirection();
        ChangeRiggingOverTime();
    }

    private void ChangeGrounded(bool grounded)
    {
        _animator.SetBool(_groundedBool,grounded);
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

    private void ChangeRiggingOverTime()
    {
        if (_headAim.weight != _headAimWeight)
        {
            _headAim.weight = Mathf.Lerp(_headAim.weight, _headAimWeight, Time.deltaTime * _headAimWeightLerpSpeed);
        }
    }

    public void SetHeadAimWeight(float weight)
    {
        _headAimWeight = weight;
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

    
}
