using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPotion : Consumable
{
    public override bool CheckIfCanConsume(PlayerCombatSystem playerCombatSystem)
    {
        return !playerCombatSystem.GetPlayerRefs.GetPlayerAcidation.IsAcidMax;
    }

    public override void Consume(PlayerCombatSystem playerCombatSystem)
    {
        base.Consume(playerCombatSystem);
        playerCombatSystem.GetPlayerRefs.GetPlayerAcidation.SetAcidationToMax();
        FinishConsumable();
    }

    public override void FinishConsumable()
    {
        
    }
}
