using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnableConsumable", order = 1)]

public class SpawnableConsumable : ScriptableObject
{
    public GameObject SpawnablePrefab;
    public ConsumableTypes ConsumableType;

}
