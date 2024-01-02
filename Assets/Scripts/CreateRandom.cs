using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandom : MonoBehaviour
{
    [SerializeField] List<ScriptableObject> _spawnables;
    [SerializeField] List<Transform> _spawnPoints = new List<Transform>();

    PuzzleRooms _puzzleRooms;

    TunelVariants _tunelVariants;
    List<SpawnableGeneral> _tunelList;

    private void OnEnable()
    {
        foreach(var spawnable in _spawnables)
        {
            if (spawnable != null && spawnable is TunelVariants)
            {
                _tunelVariants = (TunelVariants)spawnable;
                _tunelList = _tunelVariants.TunelVariantsList;
                SetupTunels(_tunelVariants.NumOfSpawningObjects);
            }
            if (spawnable != null && spawnable is PuzzleRooms)
            {
                _puzzleRooms = (PuzzleRooms)spawnable;
                SetupRooms(_puzzleRooms.NumOfSpawningObjects);
            }
        }
        
    }
    private void SetupRooms(int spawnablesCount) // Chcem aby som mohol spawn˙ù presn˝ poËet miestnostÌ, ktor˝ si urËÌm v scriptableObjecte. na jednom mieste.
    {
        List<GameObject> _spawnObjects = new List<GameObject>();
        if (_puzzleRooms == null || spawnablesCount == 0) return;

        int rngNumber = RandomNumGen.Random(1, spawnablesCount);
        for (int i = 0; i <= spawnablesCount; i++)
        {
            var goToSpawn = InstantiateRandomPuzzleRoom();
            _spawnObjects.Add(goToSpawn);
        }

        foreach (var spawnable in _spawnObjects)
        {
            Instantiate(spawnable, _spawnPoints[0].position, Quaternion.identity);
        }
    }
    private void SetupTunels(int spawnablesCount) // DONE
    {
        if (_tunelList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var tunelObject = InstantiateRandomTunelRoom();
            if (tunelObject != null)
            {
                Transform spawnPoint = _spawnPoints[i % 2];
                Vector3 originalPosition = tunelObject.transform.position;
                Quaternion originalRotation = tunelObject.transform.rotation;
                GameObject spawnedTunnel = Instantiate(tunelObject, originalPosition, originalRotation);

                spawnedTunnel.transform.SetParent(spawnPoint, false);
            }
            else Debug.LogError("Tunel is not possible to load/read!");
        }
    }
    private GameObject InstantiateRandomTunelRoom()
    {
        int tunelRoomsCount = _tunelList.Count;
        int randomIndex = RandomNumGen.Random(0, tunelRoomsCount);
        SpawnableGeneral spawnableGeneral = _tunelList[randomIndex];

        List<SpawnableGeneral> updatedTunelList = new List<SpawnableGeneral>(_tunelList);
        updatedTunelList.Remove(spawnableGeneral);
        _tunelList = updatedTunelList;

        Debug.Log("Chosen Room: " + randomIndex + 1);

        if (spawnableGeneral != null) return spawnableGeneral.prefab.gameObject;
        else return null;
    }

    private GameObject InstantiateRandomPuzzleRoom()
    {
        int puzzleRoomsCount = _puzzleRooms.PuzzleRoomsList.Count;
        if (puzzleRoomsCount == 0) return null;
        int randomIndex = RandomNumGen.Random(0, puzzleRoomsCount);
        PuzzleRoom randomPuzzleRoom = _puzzleRooms.PuzzleRoomsList[randomIndex];
        Debug.Log("Chosen Room: " + randomIndex);
        if (randomPuzzleRoom != null) return randomPuzzleRoom.prefab.gameObject;
        else return null;
    }
}
