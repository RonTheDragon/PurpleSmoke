using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Iinteractable
{
    public bool CanInteract {  get; set; }
    public abstract bool Interact(PlayerInteraction playerIntercation);
}
