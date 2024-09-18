using System;
using UnityEngine;

public class PlayerGlide : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private float _glideDownSpeed;
    private CharacterController _characterController;
    private PlayerJump _playerJump;
    private PlayerGravity _playerGravity;
    private PlayerGroundCheck _playerGroundCheck;
    private PlayerAnimations _playerAnimations;
    private PlayerCombatSystem _playerCombatSystem;
    private bool _bIsGliding, _glideInput;
    public Action OnGlide;
    [SerializeField] private float _glideCooldown;
    private float _glideCD;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _playerJump = playerComponents.GetPlayerJump;
        _playerGravity = playerComponents.GetPlayerGravity;
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _playerGroundCheck.OnGroundCheckChange += (b) => StopGlide();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    public void GlideInput()
    {
        _glideInput = true;
    }

    public void StopGlideInput()
    {
        _glideInput = false;
    }

    private void StartGlide()
    {
        _glideCD = _glideCooldown; // Reset cooldown
        _playerJump.StopJumpMidAir();
        _playerGravity.AddNotFallingReason("Glide");
        _bIsGliding = true;
        _playerAnimations.ChangeGlide(true);
        OnGlide?.Invoke();
    }

    private void StopGlide()
    {
        _bIsGliding = false;
        _playerAnimations.ChangeGlide(false);
        _playerGravity.RemoveNotFallingReason("Glide");
    }

    private void PlayerUpdate()
    {
        if (_bIsGliding)
        {
            _characterController.Move(new Vector3(0, -_glideDownSpeed, 0) * Time.deltaTime);
        }

        if (_glideInput)
        {
            if (!_bIsGliding && _playerJump.CanGlide() && !_playerCombatSystem.GetIsBusyAttacking && _glideCD <= 0)
            {
                StartGlide();
            }
        }
        else
        {
            if (_bIsGliding && _glideCD <= 0)
            {
                StopGlide();
            }
        }

        if (_glideCD > 0)
        {
            _glideCD -= Time.deltaTime;
        }
    }

    public bool IsGliding()
    {
        return _bIsGliding;
    }
}
