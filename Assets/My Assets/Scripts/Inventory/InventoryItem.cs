using UnityEngine;

public abstract class InventoryItem : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _pickUpTag;

    public string GetName => _name;
    public Sprite GetSprite => _sprite;
    public string GetPickUpTag => _pickUpTag;
    public bool CanBeDropped => !string.IsNullOrEmpty(_pickUpTag);
}
