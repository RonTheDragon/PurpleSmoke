using UnityEngine;

public class PlayerJump : MonoBehaviour, IPlayerComponent
{
    private PlayerGravity _gravity;
    private PlayerGroundCheck _groundCheck;
    private Camera _camera;
    private CharacterController _characterController;
    private PlayerAnimations _playerAnimations;
    private PlayerCombatSystem _playerCombatSystem;

    [SerializeField] private float _jumpPower = 10.0f;
    [SerializeField] private float _loseJumpPowerMult = 1f;
    [SerializeField] private float _jumpMovementPower = 5;
    [SerializeField] private float _jumpCooldown = 0.2f;

    [SerializeField][ReadOnly] private float _currentJumpForce;

    private float _currentJumpCooldown;

    [SerializeField] private float _fallWhenReachVelocity = 1f;

    [SerializeField] private bool _isDoubleJump;

    private Vector2 _jumpMovementInput;

    [SerializeField] private Vector2 _currentJumpMovement;

    private bool _canJump = true;

    private bool _jumpButtonPressed;

    private void PlayerUpdate()
    {
        TryToMoveUpwards();
        JumpCooldown();
        _groundCheck.IsGrounded(); // Keep this for regular checks
    }

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _characterController = playerComponents.GetCharacterController;
        _gravity = playerComponents.GetPlayerGravity;
        _camera = playerComponents.GetCamera;
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _groundCheck = playerComponents.GetPlayerGroundCheck;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _groundCheck.OnGroundCheckChange += ResetDoubleJump;
        playerComponents.OnUpdate += PlayerUpdate;
    }

    private void TryToMoveUpwards()
    {
        if (IsJumping())
        {
            MoveUpwards();
            LoseUpwardsVelocity();
            MoveSideways();
            LoseSidewaysVelocity();
            EndJumpAndFall();
        }
    }

    private void JumpCooldown()
    {
        if (_currentJumpCooldown > 0)
        {
            _currentJumpCooldown -= Time.deltaTime;
        }
    }

    private void MoveUpwards()
    {
        _characterController.Move(new Vector3(0, _currentJumpForce * Time.deltaTime, 0));
    }

    private void MoveSideways()
    {
        _characterController.Move(new Vector3(_currentJumpMovement.x, 0, _currentJumpMovement.y) * Time.deltaTime);
    }

    private void LoseUpwardsVelocity()
    {
        _currentJumpForce -= _currentJumpForce * _loseJumpPowerMult * Time.deltaTime;
    }

    private void LoseSidewaysVelocity()
    {
        _currentJumpMovement -= _currentJumpMovement * _loseJumpPowerMult * Time.deltaTime;
    }

    private void EndJumpAndFall()
    {
        if (!IsJumping())
        {
            _currentJumpForce = 0;
            _currentJumpMovement = Vector2.zero;
            _gravity.ResetFall();
            _gravity.RemoveNotFallingReason("Jump");
        }
    }

    public void StopJumpMidAir()
    {
        _currentJumpForce = 0;
        _currentJumpMovement = Vector2.zero;
        _gravity.RemoveNotFallingReason("Jump");
        _gravity.ResetFall();
        StopCharacterController();
    }

    public void TryToInitiateJump(Vector2 movement)
    {
        if (_currentJumpCooldown <= 0 && _canJump)
        {
            _jumpMovementInput = movement.normalized;
            if (_groundCheck.IsGrounded())
            {
                InitiateJump();
                _playerAnimations.Jump();
            }
            else if (_isDoubleJump && !_playerCombatSystem.GetIsBusyAttacking)
            {
                InitiateJump();
                _isDoubleJump = false;
                _playerAnimations.Flip();
                _jumpButtonPressed = true;
            }
        }
    }

    public void ReleaseJumpButton()
    {
        _jumpButtonPressed = false;
    }

    private void InitiateJump()
    {
        StopCharacterController();
        _currentJumpForce = _jumpPower;
        _gravity.AddNotFallingReason("Jump");
        _gravity.ResetFall();
        HandleJumpMovement();
        _currentJumpCooldown = _jumpCooldown;
        _groundCheck.ForceNotGrounded(); // Instantly tell everyone we're not grounded
    }

    private void StopCharacterController()
    {
        _characterController.enabled = false;
        _characterController.enabled = true;
    }

    private void HandleJumpMovement()
    {
        if (_jumpMovementInput.magnitude > 0)
        {
            Vector3 v3 = Quaternion.Euler(0, CalculateTargetAngle(_jumpMovementInput), 0) * Vector3.forward * _jumpMovementPower;
            _currentJumpMovement = new Vector2(v3.x, v3.z);
        }
    }

    private float CalculateTargetAngle(Vector2 normalizedDirection)
    {
        return Mathf.Atan2(normalizedDirection.x, normalizedDirection.y) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
    }

    private bool IsJumping()
    {
        return _currentJumpForce > _fallWhenReachVelocity;
    }

    private void ResetDoubleJump(bool ground)
    {
        if (ground)
        {
            _isDoubleJump = true;
        }
    }

    public bool CanGlide()
    {
        return _currentJumpCooldown <= 0 && !_isDoubleJump && !_jumpButtonPressed;
    }

    public void SetCanJump(bool canJump)
    {
        _canJump = canJump;
    }
}