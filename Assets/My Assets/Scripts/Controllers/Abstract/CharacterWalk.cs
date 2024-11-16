using System.Collections.Generic;
using UnityEngine;

public class CharacterWalk : MonoBehaviour
{
    [ReadOnly][SerializeField] protected float _currentSpeed;
    [SerializeField] protected float _baseWalkingSpeed;
    [SerializeField] protected float _currentTurnSpeed = 0.1f;
    protected CharacterController _characterController;
    [ReadOnly][SerializeField] private List<string> _notMovingReasons = new List<string>();
    protected bool _canMove = true;

    public void AddNotMovingReason(string reason)
    {
        if (!_notMovingReasons.Contains(reason))
        {
            _notMovingReasons.Add(reason);
            _canMove = false;
        }
    }

    public void RemoveNotMovingReason(string reason)
    {
        if (_notMovingReasons.Contains(reason))
        {
            _notMovingReasons.Remove(reason);
        }
        if (_notMovingReasons.Count == 0)
        {
            _canMove = true;
        }
    }
}
