using UnityEngine;

public class PlayerGravity : PlayerMovement
{
    [SerializeField] private float _gravityPower = 1.0f;
    private float _currentFallingSpeed = 0f;
    private bool _bCanFall = true;
    private void Update()
    {
        TryFalling();
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
        bool Falling = false;
        if (_bCanFall)
        {
            Falling=true;
        }
        return Falling;
    }

    public void SetCanFall(bool new_bCanFall)
    {
        _bCanFall = new_bCanFall;
    }

    public void ResetFall()
    {
        _currentFallingSpeed = 0;
    }

    private void FallDown()
    {
        _characterController.Move(new Vector3(0, -_currentFallingSpeed* Time.deltaTime, 0));
    }

    private void IncreaseFallingSpeed()
    {
        _currentFallingSpeed += _gravityPower * Time.deltaTime;
    }
}
