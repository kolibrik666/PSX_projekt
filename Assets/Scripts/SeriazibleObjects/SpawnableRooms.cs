using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/SpawnableRooms", order = 1)]

public class SpawnableRooms : ScriptableObject
{
    public List<SpawnableGeneral> SpawnableRoomsList = new List<SpawnableGeneral>();
    public TypeRooms roomsType;
    public int NumOfSpawningObjects;
}
