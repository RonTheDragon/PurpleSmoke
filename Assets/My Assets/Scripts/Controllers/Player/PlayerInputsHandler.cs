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
    private PlayerAimMode _playerAimMode;
    private PlayerAcidation _playerAcidation;
    private PlayerKnockout _playerKnockout;
    private PlayerDeath _playerDeath;
    private PlayerUI _playerUI;

    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput=Vector2.zero;

    private bool _uiOpen;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerWalk = playerComponents.GetPlayerWalk;
        _playerAimMode = playerComponents.GetPlayerAimMode;
        _playerJump = playerComponents.GetPlayerJump;
        _playerLook = playerComponents.GetPlayerLook;
        _playerGlide = playerComponents.GetPlayerGlide;
        _playerAcidation = playerComponents.GetPlayerAcidation;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _playerKnockout = playerComponents.GetPlayerKnockout;
        _playerDeath = playerComponents.GetPlayerDeath;
        _playerUI = playerComponents.GetPlayerUI;


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
        if (!_uiOpen)
        {
            _movementInput = context.ReadValue<Vector2>();          
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (!_uiOpen)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
        {
            _playerJump?.TryToInitiateJump(_movementInput);
            _playerGlide?.GlideInput();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerJump?.ReleaseJumpButton();
            _playerGlide?.StopGlideInput();
            _playerKnockout?.TryToGetUp();
            _playerDeath?.TryToRespawn();
        }
    }

    public void LightAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
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
        if (context.phase == InputActionPhase.Started && !_uiOpen)
        {
            _playerCombatSystem?.OnHeavyAttack();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerCombatSystem?.OnReleaseHeavyAttack();
        }
    }

    public void AimMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
        {
            _playerAimMode?.OnAimInputDown();
        }
    }

    public void Acidation(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
        {
            _playerAcidation?.OnAcidationInputDown();
        }
    }

    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _movementInput = Vector2.zero;
            _lookInput = Vector2.zero;
            _uiOpen = _playerUI.InventoryInput();
        }
    }

    public void CloseMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _uiOpen)
        {
            CloseInventoryAbruptly();
        }
    }

    public void CloseInventoryAbruptly()
    {
        _uiOpen = false;
        _playerUI.CloseInventory();
    }

    public void UseableStatic(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
        {
            _playerCombatSystem?.OnStaticUseable();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerCombatSystem?.OnStaticUseableRelease();
        }
    }

    public void UseableDynamic(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
        {
            _playerCombatSystem?.OnDynamicUseable();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _playerCombatSystem?.OnDynamicUseableRelease();
        }
    }
}
