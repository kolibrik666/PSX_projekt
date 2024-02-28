using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/TunelVariants", order = 1)]

public class TunelVariants : ScriptableObject
{
    public List<SpawnableGeneral> TunelVariantsList = new List<SpawnableGeneral>();
    public int NumOfSpawningObjects;

}
