using UnityEngine;

public abstract class CharacterAttackMovement : MonoBehaviour
{
    protected Transform _characterBody;
    protected CharacterController _characterController;
    protected Vector3 _currentAttackMovement;
    [SerializeField] protected Vector3 _incomingAttackMovement;
    protected bool _moving;

    public void SetAndApplyMovement(Vector3 movement)
    {
        SetMovement(movement);
        ApplyMovement();
    }

    public void SetMovement(Vector3 movement)
    {
        _incomingAttackMovement = movement;
    }

    public virtual void ApplyMovement()
    {
        _currentAttackMovement = _incomingAttackMovement;
        _moving = true;
    }

    protected virtual void ApplyingMovement()
    {
        if (_currentAttackMovement.magnitude > 0.1f)
        {
            _characterController.Move(((_characterBody.forward * _currentAttackMovement.z)
                + (Vector3.up * _currentAttackMovement.y) + (_characterBody.right *
                _currentAttackMovement.x)) * Time.deltaTime);
            // Gradually reduce attack velocity over time
            _currentAttackMovement -= _currentAttackMovement * 5 * Time.deltaTime;
        }
        else if (_moving)
        {
            StopMovement();
        }
    }
    public virtual void StopMovement()
    {
        _moving = false;
        _currentAttackMovement = Vector3.zero;
    }
}
