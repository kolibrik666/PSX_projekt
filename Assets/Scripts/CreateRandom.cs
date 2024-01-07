using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandom : MonoBehaviour
{
    [SerializeField] List<ScriptableObject> _spawnables;
    [SerializeField] List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsPreRoomsRight = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsPreRoomsLeft = new List<Transform>();


    PuzzleRooms _puzzleRooms;
    TunelVariants _tunelVariants;
    TunelPreRooms _preTunelRooms;

    List<SpawnableGeneral> _spawnablesList;
    //List<SpawnableGeneral> _tunelList;

    public struct ListItem<T>
    {
        public List<T> SpawnablesList { get; set; }
        public T SpawnableGeneral { get; set; }
    }

    private void OnEnable()
    {
        foreach(var spawnable in _spawnables)
        {
            if (spawnable != null)
            {
                if (spawnable is TunelVariants)
                {
                    _tunelVariants = (TunelVariants)spawnable;
                    SetupTunels(_tunelVariants.NumOfSpawningObjects);
                }
                if (spawnable is TunelPreRooms)
                {
                    _preTunelRooms = (TunelPreRooms)spawnable;
                    SetupTunelPreRooms(_spawnPointsPreRoomsRight.Count, _spawnPointsPreRoomsRight, _preTunelRooms.TunelPreRoomsListRight);
                    SetupTunelPreRooms(_spawnPointsPreRoomsLeft.Count, _spawnPointsPreRoomsLeft, _preTunelRooms.TunelPreRoomsListLeft);

                }
                if (spawnable is PuzzleRooms)
                {
                    _puzzleRooms = (PuzzleRooms)spawnable;
                    SetupRooms(_puzzleRooms.NumOfSpawningObjects);
                }
            }
        }
        
    }

    private void SetupTunelPreRooms(int spawnablesCount, List<Transform> spawnPointsList, List<SpawnableGeneral> tunelPreRoomsList)
    {
        _spawnablesList = tunelPreRoomsList;
        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var tunelObject = InstantiateRandomSpawnable(_spawnablesList);

            if (tunelObject != null)
            {
                Transform spawnPoint = spawnPointsList[i];
                Vector3 originalPosition = tunelObject.transform.position;
                Quaternion originalRotation = tunelObject.transform.rotation;
                GameObject spawnedTunnel = Instantiate(tunelObject, originalPosition, originalRotation);

                spawnedTunnel.transform.SetParent(spawnPoint, false);
            }
            else Debug.LogError("Tunel is not possible to load/read!");
        }
    }

    private void SetupRooms(int spawnablesCount) // Chcem aby som mohol spawnú presný poèet miestností, ktorý si urèím v scriptableObjecte. na jednom mieste.
    {
        List<GameObject> _spawnObjects = new List<GameObject>();
        if (_puzzleRooms == null || spawnablesCount == 0) return;

        int rngNumber = RandomNumGen.Random(1, spawnablesCount);
        for (int i = 0; i <= spawnablesCount; i++)
        {
            var goToSpawn = InstantiateRandomSpawnable(_spawnablesList);
            _spawnObjects.Add(goToSpawn);
        }

        foreach (var spawnable in _spawnObjects)
        {
            Instantiate(spawnable, _spawnPoints[0].position, Quaternion.identity);
        }
    }
    private void SetupTunels(int spawnablesCount) // DONE
    {
        _spawnablesList = _tunelVariants.TunelVariantsList;
        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var tunelObject = InstantiateRandomSpawnable(_spawnablesList);

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

    private GameObject InstantiateRandomSpawnable(List<SpawnableGeneral> list)
    {

        ListItem<SpawnableGeneral> result = list.RemoveRandomItemFromList();
        SpawnableGeneral removedItem = result.SpawnableGeneral;
        _spawnablesList = result.SpawnablesList;
        return removedItem != null ? removedItem.prefab.gameObject : null;
    }
   
    /*
    private GameObject InstantiateRandomPuzzleRoom()
    {
        int puzzleRoomsCount = _puzzleRooms.PuzzleRoomsList.Count;
        if (puzzleRoomsCount == 0) return null;
        int randomIndex = RandomNumGen.Random(0, puzzleRoomsCount);
        PuzzleRoom randomPuzzleRoom = _puzzleRooms.PuzzleRoomsList[randomIndex];
        Debug.Log("Chosen Room: " + randomIndex);
        if (randomPuzzleRoom != null) return randomPuzzleRoom.prefab.gameObject;
        else return null;
    }*/
}
