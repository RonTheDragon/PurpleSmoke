using UnityEngine;

[CreateAssetMenu(fileName = "gamemodeData", menuName = "Data Storage/Gamemode Data")]
public class SOgamemodeSelected : ScriptableObject
{
    public bool Pvp;
    public int Mode, Amount;
}
