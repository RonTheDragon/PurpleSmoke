using UnityEngine;

public abstract class Consumable : MonoBehaviour
{
    [SerializeField] private string _effectName;
    protected PlayerCombatSystem _playerCombatSystem;
    public abstract bool CheckIfCanConsume(PlayerCombatSystem playerCombatSystem);
    public virtual void Consume(PlayerCombatSystem playerCombatSystem)
    {
        _playerCombatSystem = playerCombatSystem;
    }

    public string GetEffectName => _effectName;
    public abstract void FinishConsumable();
}
