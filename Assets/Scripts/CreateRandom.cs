using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandom : MonoBehaviour
{
    [SerializeField] PuzzleRooms _puzzleRooms;
    [SerializeField] List<Transform> _spawnPoints = new List<Transform>();
    private void OnEnable()
    {
        SetupRooms();
    }
    private void SetupRooms()
    {
        int spawnPointsCount = _spawnPoints.Count;

        if (_puzzleRooms == null || spawnPointsCount == 0) return;
        int rngNumber = RandomNumGen.Random(1, spawnPointsCount);
        GameObject goToSpawn = InstantiateRandomPuzzleRoom();

        if (goToSpawn != null)
        {
            Instantiate(goToSpawn, _spawnPoints[rngNumber].position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private GameObject InstantiateRandomPuzzleRoom()
    {
        int puzzleRoomsCount = _puzzleRooms.puzzleRooms.Count;

        if (puzzleRoomsCount == 0)
        {
            return null;
        }

        int randomIndex = RandomNumGen.Random(0, puzzleRoomsCount);
        PuzzleRoom randomPuzzleRoom = _puzzleRooms.puzzleRooms[randomIndex];

        if (randomPuzzleRoom != null && randomPuzzleRoom.prefab != null)
        {
            return randomPuzzleRoom.prefab;
        }

        return null;
    }
}
