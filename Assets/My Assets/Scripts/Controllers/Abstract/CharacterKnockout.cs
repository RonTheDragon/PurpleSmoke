using System;
using UnityEngine;

public abstract class CharacterKnockout : MonoBehaviour
{
    [SerializeField] protected float _hurtCap = 10f;
    [SerializeField] protected float _staggerCap = 30f;
    [SerializeField] protected float _stunCap = 100f;
    [SerializeField] protected float _stumbleCap = 200f;

    [SerializeField] protected float _maxStunDuration = 2f;
    [SerializeField] protected float _maxStumbleDuration = 5f;
    [SerializeField] protected float _gettingUpDuration = 1f;

    [ReadOnly][SerializeField] protected float _currentStunDuration;
    [ReadOnly][SerializeField] protected bool _stumbled;
    [ReadOnly][SerializeField] protected bool _canGetUp;
    [ReadOnly][SerializeField] protected bool _gettingUp;

    public Action<bool> OnCanGetUp;
    protected void CheckIfCapsCorrect()
    {
        if (_staggerCap < _hurtCap || _stunCap < _staggerCap)
        {
            Debug.LogWarning("Warning: Stun cap should be greater than or equal to stagger cap," +
                " and stagger cap should be greater than or equal to hurt cap.");
        }
    }

    protected void StunCheckLoop()
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
                UnStunCharacter();
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

    protected void Hurt(float knockout)
    {
        float hurtWeight = Mathf.Lerp(0f, 0.5f, knockout / _hurtCap);
        SetAnimationWeight(hurtWeight);
        PlayAnimation("Ouch");
    }

    protected void Stagger(float knockout)
    {
        float staggerWeight = Mathf.Lerp(0.5f, 1f, (knockout - _hurtCap) / (_staggerCap - _hurtCap));
        SetAnimationWeight(staggerWeight);
        PlayAnimation("Ouch");
        ClearAttacks();
    }


    protected void Stun(float knockout)
    {
        if (_currentStunDuration > 0 || _stumbled)
        {
            Stagger(_staggerCap);
            return;
        }

        SetAnimationWeight(1);
        PlayAnimation("Stun");
        StunCharacter();
        _currentStunDuration = Mathf.Lerp(0f, _maxStunDuration, (knockout - _staggerCap) / (_stunCap - _staggerCap));
    }

    protected void Stumble(float knockout)
    {
        if (_stumbled || _gettingUp)
        {
            Stagger(_staggerCap);
            return;
        }
        _stumbled = true;
        _canGetUp = false;
        _currentStunDuration = Mathf.Lerp(0f, _maxStumbleDuration, (knockout - _stunCap) / (_stumbleCap - _stunCap));
        PlayAnimation("Stumble");
        StunCharacter();
    }

    public void TryToGetUp()
    {
        if (_canGetUp && _stumbled)
        {
            _canGetUp = false;
            GetUp();
        }
    }

    protected void GetUp()
    {
        PlayAnimation("GetUp");
        _currentStunDuration = _gettingUpDuration;
        _gettingUp = true;
        _stumbled = false;
        OnCanGetUp?.Invoke(false);
    }

    protected abstract void SetAnimationWeight(float weight);

    protected abstract void PlayAnimation(string animationName);

    protected abstract void ClearAttacks();

    public virtual void StunCharacter()
    {
        ClearAttacks();
        _gettingUp = false;
        _canGetUp = false;
        OnCanGetUp?.Invoke(false);
    }

    public virtual void UnStunCharacter()
    {
        _gettingUp = false;
        _stumbled = false;
        _canGetUp = false;
    }

    public bool GetIsStumbled => _stumbled;
}
