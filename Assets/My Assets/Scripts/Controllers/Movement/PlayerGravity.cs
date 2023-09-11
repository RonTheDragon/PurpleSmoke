using UnityEngine;

public class PlayerGravity : PlayerMovement
{
    [SerializeField] private float _gravityPower = 1.0f;
    private PlayerGroundCheck _groundCheck;
    [ReadOnly][SerializeField] private float _currentFallingSpeed = 0f;
    private bool _bCanFall = true;
    private void Update()
    {
        TryFalling();
    }

    public override void SetPlayerController(PlayerController controller)
    {
        base.SetPlayerController(controller);
        _groundCheck = controller.GetPlayerGroundCheck();
        _groundCheck.OnGroundCheckChange += (b) => { ResetFall(); };
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
