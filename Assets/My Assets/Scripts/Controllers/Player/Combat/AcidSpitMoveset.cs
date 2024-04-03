using UnityEngine;

public class AcidSpitMoveset : ChargeableMoveSet
{
    private ProjectilePooler _projectilePooler;
    private PlayerAnimations _playerAnimations;
    private Transform _shooter;

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
        if (!_isCharging && _castTimeLeft <= 0)
        {
            PerformAcidSpit();
        }
    }

    public override void OnReleaseLightAttack()
    {
        // Left empty
    }

    public override void MoveSetUpdate()
    {
        base.MoveSetUpdate();
        if (_castTimeLeft > 0)
        {
            _castTimeLeft -= Time.deltaTime;
        }
    }

    private void PerformAcidSpit()
    {
       // _playerAnimations.PlayAnimation(_acidSpitAttack.AnimationName); //no animation yet
        _castTimeLeft = _acidSpitAttack.CastTime;
        _projectilePooler.SpawnFromPool(_acidSpitAttack.ProjectileTag, _shooter.position, _shooter.rotation);
        // Apply other effects of the acid spit attack
        // You can modify this part based on your game logic
    }
}

[System.Serializable]
class AcidSpitAttack
{
    public string ProjectileTag;
    public string AnimationName;
    public float CastTime;
    // Add other properties related to the acid spit attack if needed
}
