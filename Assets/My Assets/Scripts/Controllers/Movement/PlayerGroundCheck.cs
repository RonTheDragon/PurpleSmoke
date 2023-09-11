using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : PlayerMovement
{
    [SerializeField] private Vector3 GroundedPosition = Vector3.down;
    [SerializeField] private Vector3 GroundedSize = Vector3.one;
    [SerializeField] private LayerMask _jumpableLayer;
    public Action<bool> OnGroundCheckChange;
    private bool _onGround;

    public bool IsGrounded()
    {
        if (Physics.CheckBox(transform.position + GroundedPosition, GroundedSize, Quaternion.identity, _jumpableLayer))
        {
            InvokeActionOnChange(true);
            return true;
        }
        InvokeActionOnChange(false);
        return false;
    }

    private void InvokeActionOnChange(bool b)
    {
        if (_onGround != b) 
        {
            OnGroundCheckChange?.Invoke(b); _onGround = b; 
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
