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
    [SerializeField] private PlayerAnimations _playerAnimations;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerKnockback _playerKnockback;
    [SerializeField] private PlayerKnockout _playerKnockout;
    [SerializeField] private PlayerAttackMovement _playerAttackMovement;
    [SerializeField] private PlayerCombatSystem _playerCombatSystem;
    [SerializeField] private PlayerAcidation _playerAcidation;
    [SerializeField] private PlayerDeath _playerDeath;
    [SerializeField] private PlayerTeleporter _playerTeleporter;
    [SerializeField] private PlayerUI _playerUI;

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
        _playerUI.InitializePlayerComponent(this);
        _playerInputsHandler.InitializePlayerComponent(this);
        _playerWalk.InitializePlayerComponent(this);
        _playerJump.InitializePlayerComponent(this);
        _playerGravity.InitializePlayerComponent(this);
        _playerLook.InitializePlayerComponent(this);
        _playerGlide.InitializePlayerComponent(this);
        _playerAnimations.InitializePlayerComponent(this);
        _playerHealth.InitializePlayerComponent(this);
        _playerKnockback.InitializePlayerComponent(this);
        _playerAttackMovement.InitializePlayerComponent(this);
        _playerCombatSystem.InitializePlayerComponent(this);
        _playerKnockout.InitializePlayerComponent(this);
        _playerDeath.InitializePlayerComponent(this);
        _playerTeleporter.InitializePlayerComponent(this);
        _playerAcidation.InitializePlayerComponent(this);
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

    public PlayerAnimations GetPlayerAnimations()
    {
        return _playerAnimations;
    }

    public PlayerKnockback GetPlayerKnockback()
    {
        return _playerKnockback;
    }

    public PlayerCombatSystem GetPlayerCombatSystem()
    {
        return _playerCombatSystem;
    }

    public PlayerHealth GetPlayerHealth()
    {
        return _playerHealth;
    }

    public PlayerAttackMovement GetPlayerAttackMovement()
    {
        return _playerAttackMovement;
    }

    public PlayerKnockout GetPlayerKnockout()
    {
        return _playerKnockout;
    }

    public PlayerDeath GetPlayerDeath()
    {
        return _playerDeath;
    }

    public PlayerTeleporter GetPlayerTeleporter()
    {
        return _playerTeleporter;
    }

    public PlayerAcidation GetPlayerAcidation()
    {
        return _playerAcidation;
    }
}
