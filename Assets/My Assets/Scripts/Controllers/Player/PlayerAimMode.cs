using System;
using UnityEngine;

public class PlayerAimMode : MonoBehaviour , IPlayerComponent
{
    private PlayerComponentsRefrences _playerComponentsRefrences;
    private PlayerWalk _playerWalk;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerAnimations _playerAnimations;
    private Camera _camera;
    private PlayerUI _playerUI;
    private PlayerHealth _playerHealth;
    private PlayerKnockout _playerKnockout;
    [SerializeField] private float _playerAimMovementSpeedMultiplier = 0.8f;
    private Transform _shootFromObject;

    private bool _isAiming;
    private bool _lockHeadAim;

    public Action<bool> OnToggleAim;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponentsRefrences = playerComponents;
        _playerWalk = playerComponents.GetPlayerWalk;
        _camera = playerComponents.GetCamera;
        _shootFromObject = playerComponents.GetShooter;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _playerUI = playerComponents.GetPlayerUI;
        _playerHealth = playerComponents.GetPlayerHealth;
        _playerKnockout = playerComponents.GetPlayerKnockout;

        OnToggleAim += _playerWalk.SetLockOnForward;
        OnToggleAim += _playerCombatSystem.SetUsingRanged;
        OnToggleAim += _playerUI.SetAcidCrosshair;
    }

    private void PlayerUpdate()
    {
        AimShooterAtCrosshair();
    }

    private void AimShooterAtCrosshair()
    {
        if (_shootFromObject == null || _camera == null)
            return;

        Vector3 directionToHitPoint;
        // Shoot a raycast from the camera's position in its forward direction
        //RaycastHit hit;
        //if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit))
        //{
        //    // Get the direction from the shooter's position to the point where the ray hits
        //    directionToHitPoint = hit.point - _shootFromObject.position;
        //}
        //else
        //{
        //    directionToHitPoint = _camera.transform.forward * 20;
        //}

        directionToHitPoint = _camera.transform.forward * 20;

        Quaternion targetRotation = Quaternion.LookRotation(directionToHitPoint, Vector3.up);
        _shootFromObject.rotation = targetRotation;
    }



    public void OnAimInputDown()
    {
        if (_playerCombatSystem.GetIsBusyAttacking || _playerKnockout.GetIsStumbled || _playerHealth.GetIsDead) return;

        if (_isAiming)
        {
            TurnAimOff();
        }
        else
        {
            TurnAimOn();
        }
    }

    private void TurnAimOn()
    {
        OnToggleAim?.Invoke(true);
        _isAiming = true;
        _playerWalk.AddSpeedModifier("Aiming", _playerAimMovementSpeedMultiplier);
        _playerComponentsRefrences.OnUpdate += PlayerUpdate;
        _playerAnimations.SetHeadAimWeight(1);
    }

    private void TurnAimOff()
    {
        OnToggleAim?.Invoke(false);
        _isAiming = false;
        _playerWalk.RemoveSpeedModifier("Aiming");
        _playerComponentsRefrences.OnUpdate -= PlayerUpdate;
        _playerAnimations.SetHeadAimWeight(0);
    }

    public void SetLockHeadAim(bool shouldLock)
    {
        _lockHeadAim = shouldLock;
        if (!_isAiming) { return; }

        if (_lockHeadAim)
        {
            _playerAnimations.SetHeadAimWeight(0); // Disable head aim when locked
        }
        else 
        {
            _playerAnimations.SetHeadAimWeight(1); // Re-enable head aim when unlocked
        }
    }

    public bool GetIsAiming => _isAiming;
}
