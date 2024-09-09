using UnityEngine;

public class PlayerTeleporter : MonoBehaviour, IPlayerComponent
{
    CharacterController controller;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        controller = playerComponents.GetCharacterController;
    }

    public void Teleport(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }
}
