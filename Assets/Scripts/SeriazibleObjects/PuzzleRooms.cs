using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/PuzzleRooms", order = 1)]
public class PuzzleRooms : ScriptableObject
{
    public List<SpawnableGeneral> PuzzleRoomsList = new List<SpawnableGeneral>();
    public bool RandomPlaces;
    public int NumOfSpawningObjects;

    public int GetRandomRoom()
    {
        return RandomNumGen.Random(1, PuzzleRoomsList.Count);
    }
}
