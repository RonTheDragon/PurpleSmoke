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
    private float _currentCharge;

    [SerializeField] private BallThrow _ballthrow;
    [SerializeField] private ChargeableStats _chargeStats;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField][Range(10, 100)] private int _linePoints = 25;
    [SerializeField][Range(0.01f, 0.25f)] private float _timeBetweenPoints = 0.1f;
    [SerializeField] private LayerMask _lineRenderHitMask;
    [SerializeField] private GameObject _sphereRender;

    public override void UseableStart(PlayerCombatSystem playerCombatSystem,InventoryItemUI item)
    {
        base.UseableStart(playerCombatSystem, item);
        _refs = playerCombatSystem.GetPlayerRefs;
        _playerAnimations = _refs.GetPlayerAnimations;
        _playerCharging = _refs.GetPlayerCharging;
        _projectilePooler = GameManager.Instance.GetProjectilePooler;
        _playerAimMode = _refs.GetPlayerAimMode;
        _refs.GetPlayerGroundCheck.OnGroundCheckChange += (b) => ThrowBall();
    }

    private void PlayerUpdate()
    {
        DrawProjection();
    }

    private void DrawProjection()
    {
        _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints/_timeBetweenPoints)+1;
        Vector3 startpos = _clownBall.position;
        Vector3 velocity = Mathf.Lerp(_ballthrow.MinVelocity, _ballthrow.Velocity, _playerCharging.GetChargePercentage())
            * _playerAimMode.GetCamera.transform.forward  / _ballthrow.Gravity;
        int i = 0;
        _lineRenderer.SetPosition(i, startpos);
        for(float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            Vector3 point = startpos + time * velocity;
            point.y = startpos.y + velocity.y * time + (Physics.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i,point);

            Vector3 lastPos = _lineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPos, (point - lastPos).normalized, out RaycastHit hit, (point - lastPos).magnitude, _lineRenderHitMask))
            {
                _lineRenderer.SetPosition(i,hit.point);
                _lineRenderer.positionCount = i + 1;
                if (!_sphereRender.activeSelf)
                    _sphereRender.SetActive(true);
                _sphereRender.transform.position = hit.point;
                return;
            }
            else
            {
                if (_sphereRender.activeSelf)
                    _sphereRender.SetActive(false);
            }
        }
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
            _clownBall.localRotation = Quaternion.identity;
            _clownBall.gameObject.SetActive(true);
            _playerAimMode.TempAim(true);
            _refs.OnUpdate += PlayerUpdate;
        }
    }

    public override void OnRelease()
    {
        _playerAnimations.PlayAnimation(_ballthrow.Animation);
        _currentCharge = _playerCharging.GetChargePercentage();
        _playerCharging.ResetCharge(this);
        _playerCombatSystem.SetCustomAction(ThrowBall);
        _refs.OnUpdate -= PlayerUpdate;
    }

    public void ThrowBall()
    {
        if (_clownBall == null) return;
        if (_clownBall.gameObject.activeSelf == false) return;
        Projectile projectile = _projectilePooler.CreateOrSpawnFromPool(_ballthrow.BallPoolName, _clownBall.position, _playerAimMode.GetCrosshairAimAtRotation());
        _ballthrow.Charge = _currentCharge;
        projectile.SetProjectile(_refs.GetCombatRules, _ballthrow);
        _clownBall.gameObject.SetActive(false);
        _playerAimMode.TempAim(false);
        _playerCombatSystem.SpendUseable(_item);
        _sphereRender.SetActive(false);
    }

    public override void OnCancel()
    {
        ThrowBall();
        _playerAnimations.PlayAnimation("Cancel");
        _refs.OnUpdate -= PlayerUpdate;
        _sphereRender.SetActive(false);
    }
}
