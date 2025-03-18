using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Devices", menuName = "Devices")]
public class SOdeviceId : ScriptableObject
{
    public List<int> DeviceIds = new List<int>();
}
