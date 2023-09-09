using UnityEngine;

public abstract class PlayerMovement : MonoBehaviour
{
    protected PlayerController _playerController;
    protected CharacterController _characterController;
    public virtual void SetPlayerController(PlayerController controller)
    {
        _playerController = controller;
        _characterController = controller.GetCharacterController();
    } 
}
