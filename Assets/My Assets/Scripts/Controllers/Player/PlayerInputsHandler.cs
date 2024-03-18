using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsHandler : MonoBehaviour,IPlayerComponent
{
    [SerializeField] private PlayerInput _playerInput;

    private PlayerWalk _playerWalk;
    private PlayerLook _playerLook;
    private PlayerJump _playerJump;
    private PlayerGlide _playerGlide;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerKnockout _playerKnockout;

    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput=Vector2.zero;


    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerWalk = playerComponents.GetPlayerWalk();
        _playerJump = playerComponents.GetPlayerJump();
        _playerLook = playerComponents.GetPlayerLook();
        _playerGlide = playerComponents.GetPlayerGlide();
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem();
        _playerKnockout = playerComponents.GetPlayerKnockout();
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
        _playerKnockout?.TryToGetUp();
        _playerJump?.TryToInitiateJump(_movementInput);
        _playerGlide?.GlideInput();

        if (context.phase == InputActionPhase.Started)
        {
            _playerGlide?.GlideInput();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerGlide?.StopGlideInput();
        }
    }

    public void LightAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _playerCombatSystem?.OnLightAttack();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerCombatSystem?.OnReleaseLightAttack();
        }
    }

    public void HeavyAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _playerCombatSystem?.OnHeavyAttack();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerCombatSystem?.OnReleaseHeavyAttack();
        }
    }
}
