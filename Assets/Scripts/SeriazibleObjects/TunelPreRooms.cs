using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/TunelPreRooms", order = 1)]
public class TunelPreRooms : ScriptableObject
{
    public List<SpawnableGeneral> TunelPreRoomsListRight = new List<SpawnableGeneral>();
    public List<SpawnableGeneral> TunelPreRoomsListLeft = new List<SpawnableGeneral>();

    public int NumOfSpawningObjects;
}
