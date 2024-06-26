using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/Consumables", order = 1)]

public class Consumables : ScriptableObject
{
    public List<SpawnableConsumable> ConsumablesList = new List<SpawnableConsumable>();
}
