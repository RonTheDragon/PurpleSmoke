using UnityEngine;

public class PlayerGravity : MonoBehaviour, IPlayerComponent
{
    private CharacterController _characterController;
    [SerializeField] private float _gravityPower = 1.0f;
    private PlayerGroundCheck _groundCheck;
    [ReadOnly][SerializeField] private float _currentFallingSpeed = 0f;
    private bool _bCanFall = true;
    private void PlayerUpdate()
    {
        TryFalling();
    }

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController();
        _groundCheck = playerComponents.GetPlayerGroundCheck();
        _groundCheck.OnGroundCheckChange += (b) => { ResetFall(); };
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void TryFalling()
    {
        if (IsFalling())
        {
            FallDown();
            IncreaseFallingSpeed();   
        }
    }

    private bool IsFalling()
    {
        if (_bCanFall && !_groundCheck.IsGrounded())
        {
            return true;
        }
        return false;
    }

    public void SetCanFall(bool new_bCanFall)
    {
        _bCanFall = new_bCanFall;
    }

    private void FallDown()
    {
        _characterController.Move(new Vector3(0, -_currentFallingSpeed* Time.deltaTime, 0));
    }

    private void IncreaseFallingSpeed()
    {
        _currentFallingSpeed += _gravityPower * Time.deltaTime;
    }
    public void ResetFall()
    {
        _currentFallingSpeed = 0;
    }
}
