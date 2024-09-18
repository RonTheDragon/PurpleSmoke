using System;
using UnityEngine;

public class PlayerKnockout : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private float _hurtCap = 10f;
    [SerializeField] private float _staggerCap = 30f;
    [SerializeField] private float _stunCap = 100f;
    [SerializeField] private float _stumbleCap = 200f;

    [SerializeField] private float _maxStunDuration = 2f;
    [SerializeField] private float _maxStumbleDuration = 5f;
    [SerializeField] private float _gettingUpDuration = 1f;

    private PlayerAnimations _playerAnimations;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerWalk _playerWalk;
    private PlayerJump _playerJump;
    private PlayerGravity _playerGravity;
    private PlayerAimMode _playerAimMode;

    [ReadOnly][SerializeField] private float _currentStunDuration;
    [ReadOnly][SerializeField] private bool _stumbled;
    [ReadOnly][SerializeField] private bool _canGetUp;
    [ReadOnly][SerializeField] private bool _gettingUp;

    public Action<bool> OnCanGetUp;


    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerAnimations = playerComponents.GetPlayerAnimations;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _playerWalk = playerComponents.GetPlayerWalk;
        _playerJump = playerComponents.GetPlayerJump;
        _playerGravity = playerComponents.GetPlayerGravity;
        _playerAimMode = playerComponents.GetPlayerAimMode;
        playerComponents.OnUpdate += PlayerUpdate;

        // Check if caps are set correctly
        if (_staggerCap < _hurtCap || _stunCap < _staggerCap)
        {
            Debug.LogWarning("Warning: Stun cap should be greater than or equal to stagger cap," +
                " and stagger cap should be greater than or equal to hurt cap.");
        }
    }

    private void PlayerUpdate()
    { 
        if (_currentStunDuration > 0)
        {
            _currentStunDuration -= Time.deltaTime;
        }
        else if (_currentStunDuration < 0)
        {
            _currentStunDuration = 0;
            if (_stumbled)
            {
                _canGetUp = true;
                OnCanGetUp?.Invoke(true);
            }
            else
            {
                UnStunPlayer();
            }
        }
    }

    public void RecieveKnockout(float knockout)
    {
        if (knockout <= 0) { return; }

        // Check the severity of the knockout and react accordingly
        if (knockout <= _hurtCap)
        {
            // Apply hurt animation or effect
            Hurt(knockout);
        }
        else if (knockout <= _staggerCap)
        {
            // Apply stagger animation or effect
            Stagger(knockout);
        }
        else if (knockout <= _stunCap)
        {
            // Apply stun animation or effect
            Stun(knockout);
        }
        else
        {
            // Apply stumble animation or effect
            Stumble(knockout);
        }
    }

    private void Hurt(float knockout)
    {
        float hurtWeight = Mathf.Lerp(0f, 0.5f, knockout / _hurtCap);
        _playerAnimations.SetLayerWeight(2, hurtWeight);
        _playerAnimations.PlayAnimation("Ouch");
    }

    private void Stagger(float knockout)
    {
        float staggerWeight = Mathf.Lerp(0.5f, 1f, (knockout - _hurtCap) / (_staggerCap - _hurtCap));
        _playerAnimations.SetLayerWeight(2, staggerWeight);
        _playerAnimations.PlayAnimation("Ouch"); 
        _playerCombatSystem.ClearAttacks();
    }


    private void Stun(float knockout)
    {
        if (_currentStunDuration > 0 || _stumbled) 
        {
            Stagger(_staggerCap); 
            return; 
        }

        _playerAnimations.SetLayerWeight(2, 1);
        _playerAnimations.PlayAnimation("Stun");
        StunPlayer();
        _currentStunDuration = Mathf.Lerp(0f, _maxStunDuration, (knockout - _staggerCap) / (_stunCap - _staggerCap));
    }

    private void Stumble(float knockout)
    {
        if (_stumbled || _gettingUp)
        {
            Stagger(_staggerCap);
            return;
        }
        _stumbled = true;
        _canGetUp = false;
        _currentStunDuration = Mathf.Lerp(0f, _maxStumbleDuration, (knockout - _stunCap) / (_stumbleCap - _stunCap));
        _playerAnimations.PlayAnimation("Stumble");
        StunPlayer();
    }

    public void TryToGetUp()
    {
        if (_canGetUp && _stumbled)
        {
            _canGetUp = false;
            GetUp();
        }
    }

    private void GetUp()
    {
        _playerAnimations.PlayAnimation("GetUp");
        _currentStunDuration = _gettingUpDuration;
        _gettingUp = true;
        _stumbled = false;
        OnCanGetUp?.Invoke(false);
    }

    public void StunPlayer()
    {
        _gettingUp = false;
        _canGetUp = false;

        _playerCombatSystem.ClearAttacks();
        _playerWalk.SetCanMove(false);
        _playerJump.SetCanJump(false);
        _playerCombatSystem.SetCanAttack(false);
        _playerGravity.ClearNotFallingReasons();
        _playerAimMode.SetLockHeadAim(true);
    }

    public void UnStunPlayer()
    {
        _gettingUp = false;
        _stumbled = false;
        _canGetUp = false;

        _playerWalk.SetCanMove(true);
        _playerJump.SetCanJump(true);
        _playerCombatSystem.SetCanAttack(true);
        _playerAimMode.SetLockHeadAim(false);
    }

    public bool GetIsStumbled => _stumbled;
}
