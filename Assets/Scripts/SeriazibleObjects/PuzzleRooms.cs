using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PuzzleRooms", order = 1)]
public class PuzzleRooms : ScriptableObject
{
    public List<PuzzleRoom> puzzleRooms = new List<PuzzleRoom>();
  
}
