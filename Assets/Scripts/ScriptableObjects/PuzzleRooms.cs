using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/PuzzleRooms", order = 1)]
public class PuzzleRooms : ScriptableObject
{
    public List<SpawnablePuzzle> PuzzleRoomsList = new List<SpawnablePuzzle>();
}
