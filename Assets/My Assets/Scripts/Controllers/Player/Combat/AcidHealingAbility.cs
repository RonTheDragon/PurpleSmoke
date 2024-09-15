using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidHealingAbility : UseableAbility
{
    private PlayerAcidation _playerAcid;
    private PlayerHealth _playerHealth;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private float _acidPercentCost, _healthPercentHealed;
    public override void OnPress()
    {
        AcidHealing();
    }

    public override void OnRelease()
    {
        
    }

    public override void UseableStart(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
        PlayerComponentsRefrences refs = playerCombatSystem.GetPlayerRefs;
        _playerAcid = refs.GetPlayerAcidation;
        _playerHealth = refs.GetPlayerHealth;
    }

    private void AcidHealing()
    {
        if (_playerHealth.IsHealthFull) return;

        if (_playerAcid.TrySpendAcid(_playerAcid.Max /100 * _acidPercentCost))
        {
            _playerHealth.Heal(_playerHealth.MaxHP / 100 * _healthPercentHealed);
            _particle.Play();
        }    
    }
}
