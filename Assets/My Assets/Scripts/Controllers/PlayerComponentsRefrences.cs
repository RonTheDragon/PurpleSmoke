using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponentsRefrences : MonoBehaviour
{
    [SerializeField] private Transform _playerBody;
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private PlayerInputsHandler _playerInputsHandler;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerWalk _playerWalk;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private PlayerJump _playerJump;
    [SerializeField] private PlayerGravity _playerGravity;
    [SerializeField] private PlayerGroundCheck _playerGroundCheck;

    private void Start()
    {
        InitializePlayerMovements();
    }


    private void InitializePlayerMovements()
    {
        _playerInputsHandler.SetPlayerComponents(this);
        _playerWalk.SetPlayerComponents(this);
        _playerJump.SetPlayerComponents(this);
        _playerGravity.SetPlayerComponents(this);
        _playerLook.SetPlayerComponents(this);
        _playerGroundCheck.SetPlayerComponents(this);
    }

    public PlayerWalk GetPlayerWalk() 
    {
        return _playerWalk; 
    }

    public PlayerJump GetPlayerJump()
    {
        return _playerJump;
    }

    public PlayerLook GetPlayerLook()
    {
        return _playerLook;
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

    public void StopCharacterController()
    {
        _characterController.enabled = false;
        _characterController.enabled = true;
    }

}
