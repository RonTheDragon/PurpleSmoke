using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class PlayerInputsHandler : PlayerComponent
{
    [SerializeField] private PlayerInput _playerInput;
    private PlayerInputActions _actions;

    private PlayerWalk _playerWalk;
    private PlayerLook _playerLook;
    private PlayerJump _playerJump;


    public override void SetPlayerComponents(PlayerComponentsRefrences playerComponents)
    {
        base.SetPlayerComponents(playerComponents);
        _playerWalk = playerComponents.GetPlayerWalk();
        _playerJump = playerComponents.GetPlayerJump();
        _playerLook = playerComponents.GetPlayerLook();

        InitializeInputActions();
        HideMouse();
    }

    private void InitializeInputActions()
    {
        _actions = new PlayerInputActions();
        _actions.Enable();
        _actions.Player.Jump.performed += Jump;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _playerWalk.Walk(_actions.Player.Walk.ReadValue<Vector2>());
        _playerLook.Look(_actions.Player.Look.ReadValue<Vector2>());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        _playerJump.TryToInitiateJump(_actions.Player.Walk.ReadValue<Vector2>());
    }
}
