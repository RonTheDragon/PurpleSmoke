using UnityEngine;

public class PlayerGlide : MonoBehaviour , IPlayerComponent
{
    [SerializeField] private float _glideDownSpeed;
    private CharacterController _characterController;
    private PlayerJump _playerJump;
    private PlayerGravity _playerGravity;
    private PlayerGroundCheck _playerGroundCheck;
    private PlayerAnimations _playerAnimations;
    private bool _bIsGliding;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController();
        _playerJump = playerComponents.GetPlayerJump();
        _playerGravity = playerComponents.GetPlayerGravity();
        _playerAnimations = playerComponents.GetPlayerAnimations();
        _playerGroundCheck = playerComponents.GetPlayerGroundCheck();
        _playerGroundCheck.OnGroundCheckChange += (b) => StopGlide();
        playerComponents.OnUpdate += PlayerUpdate;
    }

    public void GlideInput()
    {
        if (_playerJump.CanGlide() && !_bIsGliding)
        {
                StartGlide();
        }
    }

    public void StopGlideInput()
    {
        if (_bIsGliding)
        {
            _playerGravity.SetCanFall(true);
            StopGlide();
        }
    }

    private void StartGlide()
    {
        _playerJump.StopJumpMidAir();
        _playerGravity.SetCanFall(false);
        _bIsGliding = true;
        _playerAnimations.ChangeGlide(true);
    }

    private void StopGlide()
    {
        _bIsGliding = false;
        _playerAnimations.ChangeGlide(false);
    }

    private void PlayerUpdate() 
    {
        if ( _bIsGliding )
        {
            _characterController.Move(new Vector3(0, -_glideDownSpeed, 0) * Time.deltaTime);
        }
    }

    public bool IsGliding()
    {
        return _bIsGliding;
    }
}
