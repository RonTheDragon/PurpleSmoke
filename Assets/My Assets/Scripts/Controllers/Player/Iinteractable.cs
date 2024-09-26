
public interface Iinteractable
{
    public bool CanInteract {  get; set; }
    public abstract bool Interact(PlayerInteraction playerIntercation);
}
