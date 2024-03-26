using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/POI", order = 1)]

public class POI : ScriptableObject
{
    public GameObject Poi;
    [Range(0f, 1f)]
    public float ChanceToSpawn;
    public POITypes Type;
}
