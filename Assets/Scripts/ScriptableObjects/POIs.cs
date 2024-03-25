using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/POIs", order = 1)]

public class POIs : ScriptableObject
{
    public List<POI> PoisGeneralList = new();
    public List<POI> PoisShopList = new();

}
