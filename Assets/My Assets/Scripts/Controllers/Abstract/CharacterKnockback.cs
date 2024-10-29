using UnityEngine;

public abstract class CharacterKnockback : MonoBehaviour
{
    protected CharacterController _characterController;
    protected Vector3 _knockbackVelocity;
    [SerializeField] protected float _knockbackDecelerateSpeed = 5;

    public virtual void TakeKnockback(Vector2 knockback, Vector3 attackLocation)
    {
        Vector3 knockbackDirection = (transform.position - attackLocation).normalized;
        _knockbackVelocity = new Vector3(knockbackDirection.x * knockback.x, knockback.y, knockbackDirection.z * knockback.x);
    }

    protected void KnockbackLoop()
    {
        if (_characterController != null)
        {
            if (_knockbackVelocity.magnitude > 0.1)
            {
                _characterController.Move(_knockbackVelocity * Time.deltaTime);

                // Gradually reduce knockback velocity over time
                _knockbackVelocity -= _knockbackVelocity * _knockbackDecelerateSpeed * Time.deltaTime;
            }
            else
            {
                EndKnockback();
            }
        }
    }

    protected virtual void EndKnockback()
    {
        _knockbackVelocity = Vector3.zero;
    }
}
