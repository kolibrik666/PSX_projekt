using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandom : MonoBehaviour
{
    [SerializeField] SpawnableGeneral _metalDoor;
    [SerializeField] List<ScriptableObject> _spawnables;
    [SerializeField] List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsPreRoomsRight = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsPreRoomsLeft = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsCorridors1 = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsCorridors2 = new List<Transform>();


    PuzzleRooms _puzzleRooms;
    TunelVariants _tunelVariants;
    TunelPreRooms _preTunelRooms;

    List<SpawnableGeneral> _spawnablesList;
    List<SpawnableGeneral> _spawnedCorridorList;
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

                    SetupCorridors(_spawnPointsPreRoomsRight.Count, _spawnPointsCorridors2, _preTunelRooms.TunelCorridorListRight);
                    SetupCorridors(_spawnPointsPreRoomsLeft.Count, _spawnPointsCorridors1, _preTunelRooms.TunelCorridorListLeft);

                }
                if (spawnable is PuzzleRooms)
                {
                    _puzzleRooms = (PuzzleRooms)spawnable;
                    SetupTunelPreRooms(_spawnPointsPreRoomsRight.Count, _spawnPointsPreRoomsRight, _preTunelRooms.TunelCorridorListRight);
                    SetupTunelPreRooms(_spawnPointsPreRoomsLeft.Count, _spawnPointsPreRoomsLeft, _preTunelRooms.TunelCorridorListLeft);

                    //SetupRooms(_puzzleRooms.NumOfSpawningObjects);
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
            var tunelObject = InstantiateRandomSpawnable(_spawnablesList).SpawnablePrefab;

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

    private void SetupCorridors(int spawnablesCount, List<Transform> spawnPointsList, List<SpawnableGeneral> tunelPreRoomsList)
    {
        _spawnablesList = tunelPreRoomsList;
        _spawnedCorridorList = new List<SpawnableGeneral>();

        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {         
            var objectGeneral = InstantiateRandomSpawnable(_spawnablesList);


            foreach (var spawnable in _spawnedCorridorList) //zlé sa spawnuju na súradniciach, proste pre každú jednu zvláš spawn? a porovnáš? èi už existuje taka
            {
                if (spawnable.RoomType == TypeRooms.TunelCorridorConnected && objectGeneral.RoomType == TypeRooms.TunelCorridorConnected)
                {
                    objectGeneral = _metalDoor;
                    break;
                }
                else if (spawnable.RoomType == TypeRooms.TunelCorridorConnected && objectGeneral.RoomType == TypeRooms.TunelCorridor)
                {
                    objectGeneral = _metalDoor;
                    break;
                }
                else if (objectGeneral.RoomType == TypeRooms.TunelCorridorConnected && spawnable.RoomType == TypeRooms.TunelCorridor)
                {
                    objectGeneral = _metalDoor;
                    break;
                }
                else if (objectGeneral.RoomType == TypeRooms.General) continue;
            }           
            
            _spawnedCorridorList.Add(objectGeneral);
            var tunelObject = objectGeneral.SpawnablePrefab;

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
        _spawnablesList = _puzzleRooms.PuzzleRoomsList;

        if (_puzzleRooms == null || spawnablesCount == 0) return;

        int rngNumber = RandomNumGen.Random(1, spawnablesCount);
        for (int i = 0; i < spawnablesCount; i++)
        {
            var goToSpawn = InstantiateRandomSpawnable(_spawnablesList).SpawnablePrefab;
            _spawnObjects.Add(goToSpawn);
        }

        foreach (var spawnable in _spawnObjects)
        {
            Instantiate(spawnable, _spawnPoints[0].position, Quaternion.identity);
           // spawnable.transform.SetParent(_spawnPoints[0].position, false);

        }
    }
    private void SetupTunels(int spawnablesCount) // DONE
    {
        _spawnablesList = _tunelVariants.TunelVariantsList;
        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var tunelObject = InstantiateRandomSpawnable(_spawnablesList).SpawnablePrefab;

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

    private SpawnableGeneral InstantiateRandomSpawnable(List<SpawnableGeneral> list)
    {

        ListItem<SpawnableGeneral> result = list.RemoveRandomItemFromList();
        SpawnableGeneral removedItem = result.SpawnableGeneral;
        _spawnablesList = result.SpawnablesList;
        return removedItem != null ? removedItem : null;
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
