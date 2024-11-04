using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    protected bool _canAttack = true;

    public bool GetCanAttack => _canAttack;

    public void SetCanAttack(bool canAttack)
    {
        _canAttack = canAttack;
    }

   
}
