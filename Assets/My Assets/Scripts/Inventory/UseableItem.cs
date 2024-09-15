using UnityEngine;

[CreateAssetMenu(fileName = "New Useable", menuName = "Items/Useable")]
public class UseableItem : InventoryItem
{
    [SerializeField] private UseableAbility _useable;

    public UseableAbility GetUseable => _useable;
}
