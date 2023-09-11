using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _playerBody;
    [SerializeField] private Transform _cameraHolder;
    private PlayerInput _playerInput => GetComponent<PlayerInput>();
    private PlayerInputActions _actions;
    private Camera _mainCamera => Camera.main;
    private CharacterController _characterController => GetComponent<CharacterController>();
    private PlayerWalk _playerWalk => GetComponent<PlayerWalk>();
    private PlayerLook _playerLook => GetComponent<PlayerLook>();
    private PlayerJump _playerJump => GetComponent<PlayerJump>();
    private PlayerGravity _playerGravity => GetComponent<PlayerGravity>();
    private PlayerGroundCheck _playerGroundCheck => GetComponent<PlayerGroundCheck>();

    private void Start()
    {
        InitializePlayerMovements();
        InitializeInputActions();
        HideMouse();
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
        _playerLook.SetPlayerController(this);
        _playerGroundCheck.SetPlayerController(this);
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public CharacterController GetCharacterController()
    {
        return _characterController;
    }

    public PlayerGravity GetPlayerGravity()
    {
        return _playerGravity;
    }

    public Transform GetPlayerBody()
    {
        return _playerBody;
    }

    public Camera GetCamera()
    {
        return _mainCamera;
    }

    public Transform GetCameraHolder()
    {
        return _cameraHolder;
    }

    public PlayerGroundCheck GetPlayerGroundCheck()
    {
        return _playerGroundCheck;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        _playerJump.TryToInitiateJump();
    }

    private void Update()
    {
        _playerWalk.Walk(_actions.Player.Walk.ReadValue<Vector2>());
        _playerLook.Look(_actions.Player.Look.ReadValue<Vector2>());
    }
}
