using UnityEngine;

public class CombatRules : MonoBehaviour
{
    public enum CombatMode
    {
        Team,   // Only enemies from different teams can be damaged
        Solo,   // Only the damager cannot be damaged by it
        Danger  // Damages everyone, including the damager
    }

    [SerializeField] private string _team = "";  // The team's string, empty means solo

    // Function to check if the current entity can damage the target based on the combat mode
    public bool CanDamage(GameObject target, CombatMode mode = CombatMode.Team)
    {
        if (mode == CombatMode.Danger)
            return true; // Always damages everyone, including the damager

        if (target == gameObject)
            return false; // Can't damage itself

        // Handle the solo mode where only the damager can't be damaged by it
        if (mode == CombatMode.Solo || _team == "")
            return true; // Can damage someone else

        CombatRules targetCombatRules = target.GetComponent<CombatRules>();
        if (targetCombatRules != null)
        {
            // In the team mode, only enemies from different teams can be damaged
            return targetCombatRules.GetTeam != _team;
        }
        else
        {
            return false;
        }
    }

    // Getter for the team string
    public string GetTeam => _team;
}
