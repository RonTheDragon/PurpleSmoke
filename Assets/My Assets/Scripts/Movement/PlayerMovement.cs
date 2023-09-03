using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController => GetComponent<CharacterController>();

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        
    }
}
