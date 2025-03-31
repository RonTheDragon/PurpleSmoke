using System;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private Vector3 GroundedPosition = Vector3.down;
    [SerializeField] private Vector3 GroundedSize = Vector3.one;
    [SerializeField] private LayerMask _jumpableLayer;
    [SerializeField] private float _ungroundedDelay = 0.5f;

    public Action<bool> OnGroundCheckChange;
    private bool _onGround;
    private float _ungroundedTimer = 0f;
    private bool _isPhysicallyGrounded; // This will be the "truth"

    // Public getter for the immediate physical state
    public bool IsPhysicallyGrounded => _isPhysicallyGrounded;

    public bool IsGrounded()
    {
        bool currentGroundState = Physics.CheckBox(transform.position + GroundedPosition,
            GroundedSize, Quaternion.identity, _jumpableLayer);

        _isPhysicallyGrounded = currentGroundState; // Always update the true state

        if (currentGroundState)
        {
            _ungroundedTimer = 0f;
            InvokeActionOnChange(true);
            return true;
        }
        else
        {
            if (_onGround) // Only start counting if we were previously grounded
            {
                _ungroundedTimer += Time.deltaTime;
                if (_ungroundedTimer >= _ungroundedDelay)
                {
                    InvokeActionOnChange(false);
                    return false;
                }
                return true; // Still report as grounded during delay
            }
            return false;
        }
    }

    private void InvokeActionOnChange(bool b)
    {
        if (_onGround != b)
        {
            OnGroundCheckChange?.Invoke(b);
            _onGround = b;
        }
    }

    private void OnDrawGizmos()
    {
        if (IsGrounded())
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawCube(transform.position + GroundedPosition, GroundedSize);
    }
}