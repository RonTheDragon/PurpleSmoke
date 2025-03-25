using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Devices", menuName = "Data Storage/Devices")]
public class SOdeviceId : ScriptableObject
{
    public List<int> DeviceIds = new List<int>();
}
