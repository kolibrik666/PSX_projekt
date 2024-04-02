using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/SubwayPlatforms", order = 1)]

public class SubwayPlatforms : ScriptableObject
{
    public List<SpawnableGeneral> SubwayPlatformsList = new();
}
