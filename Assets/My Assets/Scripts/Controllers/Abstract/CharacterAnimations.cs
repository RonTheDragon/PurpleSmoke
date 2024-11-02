using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAnimations : MonoBehaviour
{
    protected int _currentWalk;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected string _walkInt;

    public void ChangeWalk(int walkInt)
    {
        if (_currentWalk != walkInt)
        {
            _currentWalk = walkInt;
            _animator.SetInteger(_walkInt, walkInt);
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (animationName == string.Empty) return;

        _animator.SetTrigger(animationName);
    }

    public void SetLayerWeight(int layer, float amount)
    {
        _animator.SetLayerWeight(layer, amount);
    }
}
