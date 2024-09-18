using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGravity : MonoBehaviour, IPlayerComponent
{
    private CharacterController _characterController;
    [SerializeField] private float _gravityPower = 1.0f;
    private PlayerGroundCheck _groundCheck;
    [ReadOnly][SerializeField] private float _currentFallingSpeed = 0f;
    [ReadOnly][SerializeField] private List<string> _notFallingReasons = new List<string>();
    private bool _canFall=true;

    private void PlayerUpdate()
    {
        TryFalling();
    }

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _groundCheck = playerComponents.GetPlayerGroundCheck;
        _groundCheck.OnGroundCheckChange += (b) => { ResetFall(); };
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void TryFalling()
    {
        if (IsFalling())
        {
            FallDown();
            if (IsActuallyFalling())
            {
                IncreaseFallingSpeed();
            }
        }
    }

    private bool IsFalling()
    {
        return _canFall && !_groundCheck.IsGrounded();
    }

    private void FallDown()
    {
        _characterController.Move(new Vector3(0, -_currentFallingSpeed * Time.deltaTime, 0));
    }

    private void IncreaseFallingSpeed()
    {
        _currentFallingSpeed += _gravityPower * Time.deltaTime;
    }

    public bool IsActuallyFalling()
    {
        return _currentFallingSpeed < 1 || _characterController.velocity.y < 0f;
    }

    public void ResetFall()
    {
        _currentFallingSpeed = 0;
    }

    public void AddNotFallingReason(string reason)
    {
        if (!_notFallingReasons.Contains(reason))
        {
            _notFallingReasons.Add(reason);
            _canFall = false;
        }
    }

    public void RemoveNotFallingReason(string reason)
    {
        if (_notFallingReasons.Contains(reason))
        {
            _notFallingReasons.Remove(reason);
        }
        if (_notFallingReasons.Count == 0)
        {
            _canFall = true;
        }
    }

    public void ClearNotFallingReasons()
    {
        _notFallingReasons.Clear();
        _canFall = true;
    }
}
