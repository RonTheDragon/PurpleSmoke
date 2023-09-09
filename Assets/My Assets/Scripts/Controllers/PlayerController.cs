using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput => GetComponent<PlayerInput>();

    private PlayerInputActions _actions;
    private CharacterController _characterController => GetComponent<CharacterController>();
    private PlayerWalk _playerWalk => GetComponent<PlayerWalk>();
    private PlayerJump _playerJump => GetComponent<PlayerJump>();
    private PlayerGravity _playerGravity => GetComponent<PlayerGravity>();

    private void Start()
    {
        InitializePlayerMovements();
        InitializeInputActions();
    }

    private void InitializeInputActions()
    {
        _actions = new PlayerInputActions();
        _actions.Enable();
    }

    private void InitializePlayerMovements()
    {
        _playerWalk.SetPlayerController(this);
        _playerJump.SetPlayerController(this);
        _playerGravity.SetPlayerController(this);
    }

    public CharacterController GetCharacterController()
    {
        return _characterController;
    }

    public PlayerGravity GetPlayerGravity()
    {
        return _playerGravity;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        _playerJump.TryToInitiateJump();
    }

    private void Update()
    {
        _playerWalk.Walk(_actions.Player.Walk.ReadValue<Vector2>());
    }
}
