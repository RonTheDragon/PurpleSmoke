using UnityEngine;

public class PlayerAimMode : MonoBehaviour , IPlayerComponent
{
    private PlayerComponentsRefrences _playerComponentsRefrences;
    private PlayerWalk _playerWalk;
    private PlayerCombatSystem _playerCombatSystem;
    private Camera _camera;
    [SerializeField] private float _playerAimMovementSpeedMultiplier = 0.8f;
    private Transform _shootFromObject;

    private bool _isAiming;
    
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponentsRefrences = playerComponents;
        _playerWalk = playerComponents.GetPlayerWalk();
        _camera = playerComponents.GetCamera();
        _shootFromObject = playerComponents.GetShooter();
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem();
    }

    private void PlayerUpdate()
    {
        AimShooterAtCameraCenter();
    }

    private void AimShooterAtCameraCenter()
    {
        if (_shootFromObject == null || _camera == null)
            return;

        // Get the direction from the shooter's position to the center of the camera
        Vector3 directionToCameraCenter = _camera.transform.position - _shootFromObject.position;

        // Ignore vertical component to keep the shooter's orientation parallel to the ground
        directionToCameraCenter.y = 0f;

        // Rotate the shooter to face the camera center direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToCameraCenter, Vector3.up);
        _shootFromObject.rotation = targetRotation;
    }


    public void OnAimInputDown()
    {
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
        _isAiming = true;
        _playerWalk.SetLockOnForward(true);
        _playerWalk.AddSpeedModifier("Aiming", 0.8f);
        _playerComponentsRefrences.OnUpdate += PlayerUpdate;
        _playerCombatSystem.SetUsingRanged(true);
    }

    private void TurnAimOff()
    {
        _isAiming = false;
        _playerWalk.SetLockOnForward(false);
        _playerWalk.RemoveSpeedModifier("Aiming");
        _playerComponentsRefrences.OnUpdate -= PlayerUpdate;
        _playerCombatSystem.SetUsingRanged(false);
    }

    public bool GetIsAiming()
    {
        return _isAiming;
    }

}
