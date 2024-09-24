using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class ConsumableItem : InventoryItem
{
    [SerializeField] private Consumable _consumable;

    public Consumable GetConsumable => _consumable;
}
