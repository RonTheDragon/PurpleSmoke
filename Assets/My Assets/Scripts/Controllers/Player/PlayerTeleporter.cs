using UnityEngine;

public class PlayerTeleporter : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private CharacterController controller;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        
    }

    public void Teleport(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }
}
