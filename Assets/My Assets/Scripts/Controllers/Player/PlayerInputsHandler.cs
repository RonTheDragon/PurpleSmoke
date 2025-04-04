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
    private PlayerInventory _playerInventory;
    private PlayerItemDropping _playerItemDropping;
    private PlayerEquipUI _playerEquipUI;

    private InputActionMap _actionMap;

    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput=Vector2.zero;

    private bool _uiOpen, _isUsingController;

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
        _playerInventory = playerComponents.GetPlayerInventory;
        _playerItemDropping = playerComponents.GetPlayerItemDropping;
        _playerEquipUI = playerComponents.GetPlayerEquipUI;
        _isUsingController = _playerInput.currentControlScheme == "Controller";

        playerComponents.OnUpdate += PlayerUpdate;

        HideMouse();
    }

    public bool UsingController => _isUsingController;

    public string GetBinding(string name)
    {
        if (_actionMap == null) _actionMap = _playerInput.actions.FindActionMap("Player");


        return TrimBindingDisplayString(_actionMap.FindAction(name).GetBindingDisplayString());
    }

    private string TrimBindingDisplayString(string displayString)
    {
        // Remove "press " if it exists
        displayString = displayString.Replace("Press ", "");

        // Check if the displayString contains " |"
        int index = displayString.IndexOf(" |");
        if (index != -1)
        {
            return displayString.Substring(0, index);
        }
        return displayString;
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

    public void DropItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _uiOpen)
        {
            _playerItemDropping.PressDropItem();
        }
        else if(context.phase == InputActionPhase.Canceled && _uiOpen)
        {
            _playerItemDropping.ReleaseDropItem();
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _uiOpen)
        {
            _playerInventory.InteractInput();
        }
    }

    public void Slot1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started &&!_uiOpen)
            _playerInventory.SlotInput(0);
    }

    public void Slot2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(1);
    }

    public void Slot3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(2);
    }

    public void Slot4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(3);
    }

    public void Slot5(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(4);
    }

    public void Slot6(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(5);
    }

    public void Slot7(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(6);
    }

    public void Slot8(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_uiOpen)
            _playerInventory.SlotInput(7);
    }

    public void SwitchSlots(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _playerEquipUI.SwitchShortcuts();
    }


    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _playerInventory != null)
        {
            _movementInput = Vector2.zero;
            _lookInput = Vector2.zero;
            _uiOpen = _playerInventory.InventoryInput();
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
        _playerInventory.CloseInventory();
    }

    public void PauseMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            GameManager.Instance.OpenPauseMenu();
        }
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
