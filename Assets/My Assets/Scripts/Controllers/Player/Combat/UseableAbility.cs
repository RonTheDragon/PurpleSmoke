using UnityEngine;

public abstract class UseableAbility : MonoBehaviour
{
    protected PlayerCombatSystem _playerCombatSystem;
    protected InventoryItemUI _item;
    public abstract void OnPress();

    public abstract void OnRelease();

    public virtual void UseableStart(PlayerCombatSystem playerCombatSystem, InventoryItemUI item)
    {
        _playerCombatSystem = playerCombatSystem;
        _item = item;
    }

    public abstract void OnCancel();
}
