using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsHandler : MonoBehaviour,IPlayerComponent
{
    [SerializeField] private PlayerInput _playerInput;

    private PlayerWalk _playerWalk;
    private PlayerLook _playerLook;
    private PlayerJump _playerJump;
    private PlayerGlide _playerGlide;

    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput=Vector2.zero;


    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerWalk = playerComponents.GetPlayerWalk();
        _playerJump = playerComponents.GetPlayerJump();
        _playerLook = playerComponents.GetPlayerLook();
        _playerGlide = playerComponents.GetPlayerGlide();
        playerComponents.OnUpdate += PlayerUpdate;

        HideMouse();
    }

    private void PlayerUpdate()
    {
        _playerWalk.Walk(_movementInput);
        _playerLook.Look(_lookInput);
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Walk(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        _playerJump?.TryToInitiateJump(_movementInput);
        _playerGlide?.GlideInput();
    }

    public void StopGlide(InputAction.CallbackContext context)
    {
        _playerGlide?.StopGlideInput();
    }
}
