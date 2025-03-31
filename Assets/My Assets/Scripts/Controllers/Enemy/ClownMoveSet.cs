using UnityEngine;

public class ClownMoveSet : EnemyCombatMoveSet
{
    [SerializeField] private BallThrow _ballThrow;
    [SerializeField] private SpitFire _spitFire;
    private EnemyAnimations _enemyAnimations;
    private EnemyComponentRefrences _enemyComponentRefrences;
    private EnemyChase _enemyChase;
    private ProjectilePooler _projectilePooler;
    [SerializeField] private Transform _ballShootFrom;
    [SerializeField] private DamageTickArea _fireSpitCollider;

    private BallThrow _currentBallThrow;
    private Quaternion _currentRotation;

    public override void MoveSetStart(CombatSystem combatSystem)
    {
        base.MoveSetStart(combatSystem);
        _enemyAttacks.Add(_ballThrow);
        _enemyAttacks.Add(_spitFire);
        _enemyComponentRefrences = _enemyCombatSystem.GetEnemyComponentRefrences;
        _enemyAnimations = _enemyComponentRefrences.GetEnemyAnimations;
        _enemyChase = _enemyComponentRefrences.GetEnemyChase;
        _projectilePooler = GameManager.Instance.GetProjectilePooler;
    }

    protected override void PerformAttack(EnemyAttackData attack)
    {
        base.PerformAttack(attack);

        if (attack is BallThrow)
        {
            _currentBallThrow = (BallThrow)attack;
            _enemyAnimations.PlayAnimation(_currentBallThrow.Animation);

            Transform target = _enemyChase.GetTarget;
            Vector3 direction = (target.position - _ballShootFrom.position).normalized;
            _currentRotation = Quaternion.LookRotation(direction);
        }
        else if (attack is SpitFire)
        {
            SpitFire a = (SpitFire)attack;
            _enemyAnimations.PlayAnimation(a.Animation);
            _fireSpitCollider.SetDamage(a.Damage);
            _fireSpitCollider.SetFireDamage(a.FireDamage);
        }
    }

    public void ThrowBall()
    {
        if (_currentBallThrow == null) return; // Prevent errors if no BallThrow attack was set

        Projectile projectile = _projectilePooler.CreateOrSpawnFromPool(_currentBallThrow.BallPoolName, _ballShootFrom.position, _currentRotation);
        projectile.SetProjectile(_enemyComponentRefrences.GetCombatRules, _currentBallThrow);
    }

    [System.Serializable]
    public class BallThrow : EnemyAttackData
    {
        public string Animation;
        public string BallPoolName;
        public float Velocity, MinVelocity;
        public float Gravity;
        public float Damage;
        public float AcidDamage;
        public bool AcidUsed;
        public float FireDamage;
        public Vector2 Knockback, Knockout;
        public float Radius;
        public float GroundFire_Time, GroundFire_Damage, GroundFire_Fire;
        public float Charge = 1;
    }

    [System.Serializable]
    public class SpitFire : EnemyAttackData
    {
        public string Animation;
        public float Damage;
        public float FireDamage;
    }
}
