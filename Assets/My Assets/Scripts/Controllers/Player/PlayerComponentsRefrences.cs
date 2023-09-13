using System;
using UnityEngine;

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
    [SerializeField] private PlayerGlide _playerGlide;

    public Action OnUpdate;

    private void Start()
    {
        InitializePlayerComponents();
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }


    private void InitializePlayerComponents()
    {
        _playerInputsHandler.InitializePlayerComponent(this);
        _playerWalk.InitializePlayerComponent(this);
        _playerJump.InitializePlayerComponent(this);
        _playerGravity.InitializePlayerComponent(this);
        _playerLook.InitializePlayerComponent(this);
        _playerGlide.InitializePlayerComponent(this);
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

    public PlayerGlide GetPlayerGlide()
    {
        return _playerGlide;
    }
}
