using UnityEngine;

public class CharacterWalk : MonoBehaviour
{
    [ReadOnly][SerializeField] protected float _currentSpeed;
    [SerializeField] protected float _baseWalkingSpeed;
    [SerializeField] protected float _currentTurnSpeed = 0.1f;
    protected CharacterController _characterController;
    protected bool _canMove = true;

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
}
