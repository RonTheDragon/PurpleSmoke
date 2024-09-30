using System;
using UnityEngine;

public class PlayerComponentsRefrences : MonoBehaviour
{
    [SerializeField] private Transform _playerBody;
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _shooter;
    [SerializeField] private PlayerInputsHandler _playerInputsHandler;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerWalk _playerWalk;
    [SerializeField] private PlayerAimMode _playerAimMode;
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
    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private PlayerUI _playerUI;
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private PlayerItemDropping _playerItemDropping;

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
        _playerAimMode.InitializePlayerComponent(this);
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
        _playerInteraction.InitializePlayerComponent(this);
        _playerInventory.InitializePlayerComponent(this);
        _playerItemDropping.InitializePlayerComponent(this);
    }

    public PlayerWalk GetPlayerWalk => _playerWalk;
    public PlayerJump GetPlayerJump => _playerJump;
    public PlayerLook GetPlayerLook => _playerLook;
    public CharacterController GetCharacterController => _characterController;
    public PlayerGravity GetPlayerGravity => _playerGravity;
    public Transform GetPlayerBody => _playerBody;
    public Camera GetCamera => _mainCamera;
    public Transform GetShooter => _shooter;
    public Transform GetCameraHolder => _cameraHolder;
    public PlayerGroundCheck GetPlayerGroundCheck => _playerGroundCheck;
    public PlayerGlide GetPlayerGlide => _playerGlide;
    public PlayerInputsHandler GetPlayerInputsHandler => _playerInputsHandler;
    public PlayerAnimations GetPlayerAnimations => _playerAnimations;
    public PlayerKnockback GetPlayerKnockback => _playerKnockback;
    public PlayerCombatSystem GetPlayerCombatSystem => _playerCombatSystem;
    public PlayerHealth GetPlayerHealth => _playerHealth;
    public PlayerAttackMovement GetPlayerAttackMovement => _playerAttackMovement;
    public PlayerKnockout GetPlayerKnockout => _playerKnockout;
    public PlayerDeath GetPlayerDeath => _playerDeath;
    public PlayerTeleporter GetPlayerTeleporter => _playerTeleporter;
    public PlayerAcidation GetPlayerAcidation => _playerAcidation;
    public PlayerAimMode GetPlayerAimMode => _playerAimMode;
    public PlayerInteraction GetPlayerInteraction => _playerInteraction;
    public PlayerUI GetPlayerUI => _playerUI;
    public PlayerInventory GetPlayerInventory => _playerInventory;
    public PlayerItemDropping GetPlayerItemDropping => _playerItemDropping;
}
