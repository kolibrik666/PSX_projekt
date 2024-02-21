using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRandom : MonoBehaviour
{
    [SerializeField] SpawnableGeneral _metalDoor;
    [SerializeField] SpawnableGeneral _placeholder;

    [SerializeField] List<ScriptableObject> _spawnables;
    [SerializeField] List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsPreRoomsRight = new List<Transform>();
    [SerializeField] List<Transform> _spawnPointsPreRoomsLeft = new List<Transform>();

    [SerializeField] List<Transform> _spawnPointsCorridorsA = new List<Transform>(); //sideA
    [SerializeField] List<Transform> _spawnPointsCorridorsB = new List<Transform>(); //sideB

    PuzzleRooms _puzzleRooms;
    TunelVariants _tunelVariants;
    TunelPreRooms _preTunelRooms;

    List<SpawnableGeneral> _spawnablesList;
    List<SpawnableGeneral> _spawnedCorridorList;

    List<SpawnableGeneral> _allDoors = new List<SpawnableGeneral>(); // vyberie s ajeden z t˝chto ako exit

    public struct ListItem<T>
    {
        public List<T> SpawnablesList { get; set; }
        public T SpawnableGeneral { get; set; }
    }
    //vyberie sa dopredu ake miestnosti sa spawnu a potom sa to poöle Ôalej?

    private void OnEnable()
    {
        _spawnedCorridorList = new List<SpawnableGeneral>();
        SpawnSpawnables();
    }

    private void SpawnSpawnables()
    {
        foreach (var spawnable in _spawnables)
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
                    SetupCorridors(_spawnPointsPreRoomsRight.Count, _spawnPointsCorridorsA, _preTunelRooms.TunelCorridorListRight);
                    SetupCorridors(_spawnPointsPreRoomsLeft.Count, _spawnPointsCorridorsB, _preTunelRooms.TunelCorridorListLeft, true);
                }
                if (spawnable is PuzzleRooms)
                {
                    _puzzleRooms = (PuzzleRooms)spawnable;
                    //SetupTunelPreRooms(_spawnPointsPreRoomsRight.Count, _spawnPointsPreRoomsRight, _preTunelRooms.TunelCorridorListRight);
                    //SetupTunelPreRooms(_spawnPointsPreRoomsLeft.Count, _spawnPointsPreRoomsLeft, _preTunelRooms.TunelCorridorListLeft);

                    //SetupRooms(_puzzleRooms.NumOfSpawningObjects);
                }
            }
        }
    }
    private void SetupCorridors(int spawnablesCount, List<Transform> spawnPointsList, List<SpawnableGeneral> tunelPreRoomsList, bool isCorridorsASpawned = false) 
    {
        // pottrebujem spawnuù na P L suradnice len z P na P a z L na L, a porovnaù Ëi jedne z nich je large
        _spawnablesList = tunelPreRoomsList;

        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var objectGeneral = InstantiateRandomSpawnable(_spawnablesList);
            if (isCorridorsASpawned) //  porovnaù iba 0(0) s 0t˝m(2) a 1(1) s 1prv˝m(3)
            {               
                var spawnable = _spawnedCorridorList[i];
                if (objectGeneral.RoomType == TypeRooms.TunelCorridorConnected && spawnable.RoomType == TypeRooms.TunelCorridorConnected ||
                    objectGeneral.RoomType == TypeRooms.TunelCorridor && spawnable.RoomType == TypeRooms.TunelCorridorConnected)
                {
                    objectGeneral = _placeholder; // NIC tu chcem alebo OPENABLE DOORS
                }
                if (objectGeneral.RoomType == TypeRooms.TunelCorridorConnected && spawnable.RoomType == TypeRooms.TunelCorridor)
                {
                    objectGeneral = _metalDoor; // EXIT alebo spawnableL
                    _allDoors.Add(objectGeneral);
                }               
            }
            _spawnedCorridorList.Add(objectGeneral);
            var tunelObject = objectGeneral.SpawnablePrefab;
            SpawnObject(tunelObject, spawnPointsList[i]);
        }
    }

    private void SetupTunelPreRooms(int spawnablesCount, List<Transform> spawnPointsList, List<SpawnableGeneral> tunelPreRoomsList)
    {
        _spawnablesList = tunelPreRoomsList;
        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var tunelObject = InstantiateRandomSpawnable(_spawnablesList).SpawnablePrefab;
            SpawnObject(tunelObject, spawnPointsList[i]);
        }
    }
    
    private void SetupTunels(int spawnablesCount) // DONE
    {
        _spawnablesList = _tunelVariants.TunelVariantsList;
        if (_spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            var tunelObject = InstantiateRandomSpawnable(_spawnablesList).SpawnablePrefab;
            SpawnObject(tunelObject, _spawnPoints[i % 2]);
        }
    }
    private void SpawnObject(GameObject toSpawn, Transform spawnPoint)
    {
        if (toSpawn != null)
        {
            Vector3 originalPosition = toSpawn.transform.position;
            Quaternion originalRotation = toSpawn.transform.rotation;
            GameObject spawnedObject = Instantiate(toSpawn, originalPosition, originalRotation);
            spawnedObject.transform.SetParent(spawnPoint, false);
        }
        else Debug.LogError("Object is not possible to load/read!");
    }
    public SpawnableGeneral InstantiateRandomSpawnable(List<SpawnableGeneral> list)
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

    /*
    private void SetupRooms(int spawnablesCount) // Chcem aby som mohol spawn˙ù presn˝ poËet miestnostÌ, ktor˝ si urËÌm v scriptableObjecte. na jednom mieste.
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
    }*/
}
