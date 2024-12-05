using UnityEngine;

public abstract class CombatMoveSet : MonoBehaviour
{
    protected float _castTimeLeft;

    public abstract void ResetAttacks(); // Resets the attack state

    // Called to initialize the combat system reference
    public abstract void MoveSetStart(CombatSystem combatSystem);

    // Method for updating the move set logic
    public abstract void MoveSetUpdate();
    public abstract class AttackData { }
}
