using UnityEngine;

public abstract class Pickup : MonoBehaviour , Iinteractable
{
    private PlayerInteraction _playerIntercation;
    [SerializeField] private bool _isInteractable;
    [SerializeField] private Collider _trigger;
    [SerializeField] private Rigidbody _rigidbody;

    public bool CanInteract { get => _isInteractable; set => _isInteractable = value; }

    protected void OnTriggerEnter(Collider other)
    {
        if (CanInteract) return;

        if (other.tag == "Player")
        {
            _playerIntercation = other.GetComponent<PlayerInteraction>();
            if (_playerIntercation != null)
            {
                Interact(_playerIntercation);
            }
        }
    }

    protected void DisableItem()
    {
        gameObject.SetActive(false);
    }

    public virtual void Spawn(float spawnProtection)
    {
        _trigger.enabled = false;
        Invoke(nameof(EnableCollider), spawnProtection);
    }

    private void EnableCollider()
    {
        _trigger.enabled = true;
    }

    public abstract bool Interact(PlayerInteraction playerIntercation);

    public Rigidbody GetRigidbody => _rigidbody;
}
