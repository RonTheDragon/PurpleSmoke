using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsHandler : MonoBehaviour,IPlayerComponent
{
    [SerializeField] private PlayerInput _playerInput;
    private PlayerInputActions _actions;

    private PlayerWalk _playerWalk;
    private PlayerLook _playerLook;
    private PlayerJump _playerJump;
    private PlayerGlide _playerGlide;


    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerWalk = playerComponents.GetPlayerWalk();
        _playerJump = playerComponents.GetPlayerJump();
        _playerLook = playerComponents.GetPlayerLook();
        _playerGlide = playerComponents.GetPlayerGlide();
        playerComponents.OnUpdate += PlayerUpdate;

        InitializeInputActions();
        HideMouse();
    }

    private void InitializeInputActions()
    {
        _actions = new PlayerInputActions();
        _actions.Enable();
        _actions.Player.Jump.performed += Jump;
        _actions.Player.StopGlide.performed += StopGlide;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PlayerUpdate()
    {
        _playerWalk.Walk(_actions.Player.Walk.ReadValue<Vector2>());
        _playerLook.Look(_actions.Player.Look.ReadValue<Vector2>());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        _playerJump.TryToInitiateJump(_actions.Player.Walk.ReadValue<Vector2>());
        _playerGlide.GlideInput();
    }

    public void StopGlide(InputAction.CallbackContext context)
    {
        _playerGlide.StopGlideInput();
    }
}
