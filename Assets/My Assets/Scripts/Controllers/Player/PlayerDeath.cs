using System;
using UnityEngine;

public class PlayerDeath : CharacterDeath, IPlayerComponent
{
    private PlayerHealth _health;
    private PlayerKnockout _playerKnockout;
    private PlayerAnimations _playerAnimations;
    private PlayerTeleporter _playerTeleporter;

    private Vector3 _respawnLocation;
    [SerializeField] private float _respawnTime;
    private float _respawnTimeLeft;
    private int _respawnCountdown;
    private bool _canRespawn;

    PlayerComponentsRefrences _playerComponents;

    public Action<int> OnRespawnCountdown;
    public Action OnDeath;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _health = playerComponents.GetPlayerHealth;
        _playerKnockout = playerComponents.GetPlayerKnockout;
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _playerTeleporter = playerComponents.GetPlayerTeleporter;

        _respawnLocation = transform.position;

        _playerComponents = playerComponents;
    }

    public override void Die()
    {
        _playerKnockout.StunCharacter();
        _playerAnimations.PlayAnimation("Death");

        _respawnTimeLeft = _respawnTime;
        _respawnCountdown = (int)_respawnTime;
        OnRespawnCountdown?.Invoke(_respawnCountdown);

        _playerComponents.OnUpdate += PlayerUpdate;
        OnDeath?.Invoke();
    }

    private void PlayerUpdate()
    {
        if (_respawnTimeLeft > 0)
        {
            _respawnTimeLeft -= Time.deltaTime;
            if (_respawnCountdown > _respawnTimeLeft+1)
            {
                _respawnCountdown--;
                OnRespawnCountdown?.Invoke(_respawnCountdown);
            }
        }
        else if (_respawnTimeLeft < 0)
        {
            _respawnTimeLeft = 0;
            _canRespawn = true;
            _playerComponents.OnUpdate -= PlayerUpdate;
        }
    }

    public void TryToRespawn()
    {
        if (_canRespawn)
        {
            Respawn();
            _canRespawn = false;
            OnRespawnCountdown?.Invoke(-1);
        }
    }

    public void Respawn()
    {
        _playerTeleporter.Teleport(_respawnLocation);
        Revive();
    }

    public override void Revive()
    {
        _health.HealToMax();
        _playerKnockout.UnStunCharacter();
        _playerAnimations.PlayAnimation("Revive");
    }
}
