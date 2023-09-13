using UnityEngine;

public abstract class PlayerComponent : MonoBehaviour
{
    protected PlayerComponentsRefrences _playerComponents;
    protected CharacterController _characterController;
    public virtual void SetPlayerComponents(PlayerComponentsRefrences playerComponents)
    {
        _playerComponents = playerComponents;
        _characterController = playerComponents.GetCharacterController();
    } 
}
