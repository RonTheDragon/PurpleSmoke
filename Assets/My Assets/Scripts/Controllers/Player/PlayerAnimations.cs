using UnityEngine;

public class PlayerAnimations : MonoBehaviour, IPlayerComponent
{
    private PlayerGroundCheck _playerGroundCheck;
    private int _currentWalk;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _walkInt;
    [SerializeField] private string _jumpTrigger;
    [SerializeField] private string _flipTrigger;
    [SerializeField] private string _groundedBool;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck();
        _playerGroundCheck.OnGroundCheckChange += ChangeGrounded;
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
