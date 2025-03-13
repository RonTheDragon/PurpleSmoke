using UnityEngine;
using static ClownMoveSet;

public class ClownBallThrowable : UseableAbility
{
    private PlayerAnimations _playerAnimations;
    private PlayerCharging _playerCharging;
    [SerializeField] private Transform _clownBall;
    private ProjectilePooler _projectilePooler;
    private PlayerAimMode _playerAimMode;
    private PlayerComponentsRefrences _refs;

    [SerializeField] private BallThrow _ballthrow;
    [SerializeField] private ChargeableStats _chargeStats;

    public override void UseableStart(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
        _refs = playerCombatSystem.GetPlayerRefs;
        _playerAnimations = _refs.GetPlayerAnimations;
        _playerCharging = _refs.GetPlayerCharging;
        _projectilePooler = GameManager.Instance.GetProjectilePooler;
        _playerAimMode = _refs.GetPlayerAimMode;
        
    }

    public override void OnPress()
    {
        if (!_playerCombatSystem.GetIsBusyAttacking)
        {
            _playerCharging.PerformCharging(_chargeStats, OnRelease);
           // _playerCharging.ResetCharge();
            _playerCharging.ActivateCharge(this);
            //_playerAnimations.PlayAnimation(ChargeThrowAnim);
            _clownBall.SetParent(_playerCombatSystem.GetRightHand);
            _clownBall.localPosition = Vector3.zero;
            _clownBall.gameObject.SetActive(true);
            _playerAimMode.TempAim(true);
        }
    }

    public override void OnRelease()
    {
        _playerAnimations.PlayAnimation(_ballthrow.Animation);
        _playerCharging.ResetCharge(this);
        _playerCombatSystem.SetCustomAction(ThrowBall);
    }

    public void ThrowBall()
    {
        Projectile projectile = _projectilePooler.CreateOrSpawnFromPool(_ballthrow.BallPoolName, _clownBall.position, _playerAimMode.GetCrosshairAimAtRotation());
        projectile.SetProjectile(_refs.GetCombatRules, _ballthrow);
        _clownBall.gameObject.SetActive(false);
        _playerAimMode.TempAim(false);
    }

    public override void OnCancel()
    {
        ThrowBall();
        _playerAnimations.PlayAnimation("Cancel");
    }
}
