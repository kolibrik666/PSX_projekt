using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/PuzzleRooms", order = 1)]
public class PuzzleRooms : ScriptableObject
{
    public List<PuzzleRoom> PuzzleRoomsList = new List<PuzzleRoom>();
    public bool RandomPlaces;
    public int NumOfSpawningObjects;

    public int getRandomRoom()
    {
        return RandomNumGen.Random(1, PuzzleRoomsList.Count);
    }
}
