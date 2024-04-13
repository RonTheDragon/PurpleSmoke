using UnityEngine;

public class AcidSpitMoveset : ChargeableMoveSet
{
    private ProjectilePooler _projectilePooler;
    private PlayerAnimations _playerAnimations;
    private Transform _shooter;
    private bool _spittingAcid;

    [SerializeField] private AcidSpitAttack _acidSpitAttack;

    public override void MoveSetStart(PlayerCombatSystem playerCombatSystem)
    {
        _projectilePooler = GameManager.Instance.ProjectilePooler;
        base.MoveSetStart(playerCombatSystem);
        _playerAnimations = playerCombatSystem.GetAnimations();
        _shooter = playerCombatSystem.GetShooter();
    }

    public override void OnLightAttack()
    {
        _spittingAcid = true;
    }

    public override void OnReleaseLightAttack()
    {
        _spittingAcid = false;
    }

    public override void MoveSetUpdate()
    {
        base.MoveSetUpdate();

        if (_spittingAcid) 
        {
            TrySpitAcid(); 
        }

        if (_castTimeLeft > 0)
        {
            _castTimeLeft -= Time.deltaTime;
        }
    }

    private void TrySpitAcid()
    {
        if (!_isCharging && _castTimeLeft <= 0)
        {
            PerformAcidSpit();
        }
    }

    private void PerformAcidSpit()
    {
       // _playerAnimations.PlayAnimation(_acidSpitAttack.AnimationName); //no animation yet
        _castTimeLeft = _acidSpitAttack.CastTime;
        Projectile projectile =_projectilePooler.SpawnFromPool(_acidSpitAttack.ProjectileTag, _shooter.position, _shooter.rotation);

        projectile.SetProjectile(transform.parent.gameObject,_acidSpitAttack);
        // Apply other effects of the acid spit attack
        // You can modify this part based on your game logic
    }

    public override void ResetAttacks()
    {
        base.ResetAttacks();
        _spittingAcid = false;
    }
}

public class ProjectileAttack
{

}

[System.Serializable]
public class AcidSpitAttack : ProjectileAttack
{
    public string ProjectileTag;
    public string AnimationName;
    public float CastTime;
    public float ProjectileSpeed;
    public float ProjectileGravity;
    public float ProjectileDamage;
    public float ProjectileAcidDamage;
    public float ProjectileExplosionRange;
    // Add other properties related to the acid spit attack if needed
}
