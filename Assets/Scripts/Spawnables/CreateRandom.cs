using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Zenject;
using System.Linq;
using System;
using Unity.VisualScripting;

public class CreateRandom : MonoBehaviour
{
    [Inject] GameStartData _gameStartData;
    [Inject] SpawnPOI.Factory _spawnPOIFactory = default;
    [Inject] TrezorPuzzleRoom.Factory _trezorPuzzleFactory = default;
    [Inject] SpotlightPuzzleRoom.Factory _spotlightPuzzleFactory = default;

    [Inject] ZenjectUtils _zenjectUtils;

    [SerializeField] NavMeshSurface _navMeshSurface;

    [SerializeField] SpawnableGeneral _metalDoor;
    [SerializeField] GameObject _doorForCorridors;
    [SerializeField] GameObject _fillerRoom;
    [SerializeField] SpawnableGeneral _placeholder;

    [SerializeField] Consumables _consumables;

    [SerializeField] List<ScriptableObject> _spawnables;
    [SerializeField] List<Transform> _spawnPoints = new();
    [SerializeField] List<Transform> _spawnPointsPreRoomsRight = new();
    [SerializeField] List<Transform> _spawnPointsPreRoomsLeft = new();

    [SerializeField] List<Transform> _spawnPointsCorridorsA = new(); //sideA
    [SerializeField] List<Transform> _spawnPointsCorridorsB = new(); //sideB

    SubwayPlatforms _subwayPlatforms;
    PuzzleRooms _puzzleRooms;
    TunelVariants _tunelVariants;
    TunelPreRooms _preTunelRooms;
    POIs _pois;

    readonly List<GameObject> _allDoors = new(); // vyberie s ajeden z t�chto ako exit

    List<SpawnableGeneral> _spawnedCorridorList;
    List<GameObject> _spawnedTunelsList = new();
    List<GameObject> _spawnedCorridorsObjects = new(); // miestnosti naspawnovane v scene
    List<GameObject> _spawnedPuzzleRoomsList = new();
    List<SpawnableGeneral> _spawnedPuzzleSpawnableList = new();
    List<GameObject> _spawnedPOIsObjects = new();
    public struct ListItem<T>
    {
        public List<T> SpawnablesList { get; set; }
        public T Spawnable { get; set; }
    }
    //vyberie sa dopredu ake miestnosti sa spawnu a potom sa to po�le �alej?


    private void OnEnable()
    {
        _spawnedCorridorList = new List<SpawnableGeneral>();
        SpawnSpawnables();
        _navMeshSurface.BuildNavMesh();
    }
    #region SpawnSpawnables
    private void SpawnSpawnables()
    {
        foreach (var spawnable in _spawnables)
        {
            switch(spawnable)
            {
                case SubwayPlatforms:
                    _subwayPlatforms = (SubwayPlatforms)spawnable;
                    SetupSubwayPlatforms();
                    break;
                case TunelVariants:
                    _tunelVariants = (TunelVariants)spawnable;
                    SetupTunels(_tunelVariants.NumOfSpawningObjects);
                    break;
                case TunelPreRooms:
                    _preTunelRooms = (TunelPreRooms)spawnable;
                    SetupTunelPreRooms(_spawnPointsPreRoomsRight.Count, _spawnPointsPreRoomsRight, _preTunelRooms.TunelPreRoomsListRight);
                    SetupTunelPreRooms(_spawnPointsPreRoomsLeft.Count, _spawnPointsPreRoomsLeft, _preTunelRooms.TunelPreRoomsListLeft);
                    SetupCorridors(_spawnPointsPreRoomsRight.Count, _spawnPointsCorridorsA, _preTunelRooms.TunelCorridorListRight);
                    SetupCorridors(_spawnPointsPreRoomsLeft.Count, _spawnPointsCorridorsB, _preTunelRooms.TunelCorridorListLeft, true);
                    break;
                case PuzzleRooms:
                    _puzzleRooms = (PuzzleRooms)spawnable;
                    SetupPuzzleRooms(_spawnedCorridorsObjects, _puzzleRooms.PuzzleRoomsList);
                    break;                   
                case POIs:
                    _pois = (POIs)spawnable;
                    SpawnPOIs(_spawnedCorridorsObjects.Concat(_spawnedTunelsList).Concat(_spawnedPuzzleRoomsList));
                    break;               
            }
        }
    }
    
    private void SetupSubwayPlatforms()
    {
        var randomPlatform = _subwayPlatforms.SubwayPlatformsList.GetRandomItemFromList();
        var spawnedObject = SpawnObjectGet(randomPlatform.SpawnablePrefab);
        spawnedObject.transform.SetParent(_spawnPoints[0], false);
    }
    #endregion
    #region POIs
    private void SpawnPOIs(IEnumerable<GameObject> fromObjects)
    {
        List<SpawnpointPOIGetter> allSpawnpointPOIs = PrepareSpawnpointsPOI(fromObjects); // v�etky s�radnice do ktor�ch sa spawn� POI
        List<POI> poisGeneralList = _pois.PoisGeneralList; // poi ktor� sa m��u spawn��
        List<POI> poisShopList = _pois.PoisShopList; // poi ktor� sa m��u spawn��

        foreach (SpawnpointPOIGetter spawnpointGetters in allSpawnpointPOIs)
        {
            foreach (var spawnpoint in spawnpointGetters.SpawnpointsList)
            {
                POI randomItem = null;
                switch (spawnpointGetters.POIType)
                {
                    case POITypes.General:
                        randomItem = poisGeneralList.GetRandomItemFromList();
                        break;
                    case POITypes.Shop:
                        randomItem = poisShopList.GetRandomItemFromList();
                        break;
                }
                var spawnedObject = SpawnObjectGet(randomItem.Poi);

                if (spawnedObject != null)
                {
                    spawnedObject.transform.SetParent(spawnpoint, false);
                    _spawnedPOIsObjects.Add(spawnedObject);
                }
            }
        }
        
        _spawnPOIFactory.Create(new()
        {
           SpawnedPOIsObjects = _spawnedPOIsObjects,
           Consumables = _consumables
        });
    }
    private List<SpawnpointPOIGetter> PrepareSpawnpointsPOI(IEnumerable<GameObject> fromObjects)
    {

        List<SpawnpointPOIGetter> allSpawnpointPOIs = new();

        var combinedCollection = fromObjects;

        foreach (var spawnedObject in combinedCollection)
        {
            SpawnpointPOIGetter[] spawnpointPOIGetters = spawnedObject.GetComponents<SpawnpointPOIGetter>();
            foreach (SpawnpointPOIGetter spawnpointPOIGetter in spawnpointPOIGetters) if(spawnpointPOIGetter != null) allSpawnpointPOIs.Add(spawnpointPOIGetter);         
            /*
            var spawnpoint = spawnedObject.GetComponent<SpawnpointPOIGetter>();
            var poiType = spawnpoint.POIType;
            if (spawnpoint != null) foreach (var spawnpointPOI in spawnpoint.SpawnpointsList)
                {
                    //spawnpoint.SpawnpointType
                    //spawnpoint.SpawnpointType
                    allSpawnpointPOIs.Add(spawnpointPOI);
                }*/
        }
        return allSpawnpointPOIs;
    }
    #endregion
    private void SetupPuzzleRooms(List<GameObject> spawnedCorridorsObjects, List<SpawnablePuzzle> puzzleRoomsList)
    {
        //potrebujem checknu� �e �i sa vybrala verzia �o m� aj PART 2 -> TypeRooms.PuzzleSplit, ak �no  tak jej prira� z toho SpawnablePuzzle niektor� �as� z ktorej bude na v�ber.
        List<SpawnablePuzzle> spawnablesList = puzzleRoomsList; // to �o sa ide spawnu�

        List<Transform> spawnpointsSideA = new();
        List<Transform> spawnpointsSideB = new();

        SpawnablePuzzle objectSplitToSpawn;
        
        foreach (var objectPuzzle in spawnedCorridorsObjects)
        {            
            var spawnpoint = objectPuzzle.GetComponent<SpawnpointRoomGetter>();
            if (spawnpoint.SpawnpointType == SpawnpointTypes.SideA) spawnpointsSideA.Add(spawnpoint.Spawnpoint);
            else spawnpointsSideB.Add(spawnpoint.Spawnpoint);
        }

        if (spawnpointsSideA.Count != 0 || spawnpointsSideB.Count != 0)
        {
            bool spawnOnSideA = RandomNumGen.Range(0, 2) == 0;
            Transform selectedSpawnpoint = spawnOnSideA ? spawnpointsSideA[0] : spawnpointsSideB[0]; // vybrany pre spawn puzzlu

            ListItem<SpawnablePuzzle> resultList = spawnablesList.RemoveRandomItemFromList();
            var objectToSpawn = resultList.Spawnable;
            spawnablesList = resultList.SpawnablesList;

            GameObject spawnedObject = null;

            switch (objectToSpawn.RoomType)
            {
                case TypeRooms.PuzzleShop:
                    spawnedObject = _trezorPuzzleFactory.Create().gameObject;
                    break;
                case TypeRooms.PuzzleGeneral:
                    spawnedObject = _spotlightPuzzleFactory.Create().gameObject;
                    break;
            }

            spawnedObject.transform.SetParent(selectedSpawnpoint.transform, false);
            //pre part2 z puzzlu potrebujem informacie o poistke �i je pickapnuta
            _spawnedPuzzleRoomsList.Add(spawnedObject);

            if (spawnOnSideA) spawnpointsSideA.Remove(selectedSpawnpoint);
            else spawnpointsSideB.Remove(selectedSpawnpoint);

            if (objectToSpawn.RoomType == TypeRooms.PuzzleSplit && objectToSpawn.SpawnablesPartsList != null) // pozrieme �i je rozpoleny alebo iba jednotny
            {
                Transform selectedOppositeSpawnpoint = !spawnOnSideA ? spawnpointsSideA[0] : spawnpointsSideB[0];
                objectSplitToSpawn = objectToSpawn.SpawnablesPartsList.GetRandomItemFromList();

                GameObject spawnedOppositeObject = SpawnObjectGet(objectSplitToSpawn.SpawnablePrefab);// spawne sa druhy puzzle
                spawnedOppositeObject.transform.SetParent(selectedOppositeSpawnpoint.transform, false);

                _spawnedPuzzleRoomsList.Add(spawnedOppositeObject);
                if (!spawnOnSideA) spawnpointsSideA.Remove(selectedOppositeSpawnpoint);
                else spawnpointsSideB.Remove(selectedOppositeSpawnpoint);
            }         
        }

        foreach (var spawnpoint in spawnpointsSideA) //vybra� random z fillerov nateraz dvere
        {
            GameObject spawnedObject = SpawnObjectGet(_fillerRoom);
            spawnedObject.transform.SetParent(spawnpoint.transform, false);
            //_allDoors.Add(spawnedObject);
        }
        foreach (var spawnpoint in spawnpointsSideB)
        {
            GameObject spawnedObject = SpawnObjectGet(_fillerRoom);
            spawnedObject.transform.SetParent(spawnpoint.transform, false);
            //_allDoors.Add(spawnedObject);
        }

        //_puzzleManager.Setup(spawnedPuzzleRoomsList); // po�leme si SpawnablePuzzle do tohto listu. alebo gameObjecty?
    }
    private void SetupCorridors(int spawnablesCount, List<Transform> spawnPointsList, List<SpawnableGeneral> tunelPreRoomsList, bool isCorridorsASpawned = false) 
    {
        // pottrebujem spawnu� na P L suradnice len z P na P a z L na L, a porovna� �i jedne z nich je large
        List<SpawnableGeneral> spawnablesList = tunelPreRoomsList;
        //_spawnpointsPuzzleRoomList = new();

        if (spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            ListItem<SpawnableGeneral> resultList = spawnablesList.RemoveRandomItemFromList();
            var objectGeneral = resultList.Spawnable;
            spawnablesList = resultList.SpawnablesList;

            if (isCorridorsASpawned) //  porovna� iba 0(0) s 0t�m(2) a 1(1) s 1prv�m(3)
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
                    //_allDoors.Add(objectGeneral);
                }               
            }
            _spawnedCorridorList.Add(objectGeneral);

            GameObject spawnedObject = SpawnObjectGet(objectGeneral.SpawnablePrefab);
            spawnedObject.transform.SetParent(spawnPointsList[i], false);
            
            if (objectGeneral.RoomType != TypeRooms.General)
            {
                var spawnPointGetter = spawnedObject.GetComponent<SpawnpointRoomGetter>();
                spawnPointGetter.SetSpawnpointType(i % 2);
                _spawnedCorridorsObjects.Add(spawnedObject);
            }
            else if (objectGeneral == _metalDoor) _allDoors.Add(spawnedObject);
        }
    }

    private void SetupTunelPreRooms(int spawnablesCount, List<Transform> spawnPointsList, List<SpawnableGeneral> tunelPreRoomsList)
    {
        List<SpawnableGeneral> spawnablesList = tunelPreRoomsList;
        if (spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            ListItem<SpawnableGeneral> resultList = spawnablesList.RemoveRandomItemFromList();
            var tunelObject = resultList.Spawnable.SpawnablePrefab;
            spawnablesList = resultList.SpawnablesList;
            SpawnObject(tunelObject, spawnPointsList[i]);
        }
    }
    
    private void SetupTunels(int spawnablesCount) // DONE
    {
        List<SpawnableGeneral> spawnablesList = _tunelVariants.TunelVariantsList;
        if (spawnablesList == null || spawnablesCount == 0) return;
        for (int i = 0; i < spawnablesCount; i++)
        {
            ListItem<SpawnableGeneral> resultList = spawnablesList.RemoveRandomItemFromList();
            var tunelObject = resultList.Spawnable.SpawnablePrefab;
            spawnablesList = resultList.SpawnablesList;
            var spawnedObj = SpawnObjectGet(tunelObject);
            spawnedObj.transform.SetParent(_spawnPoints[i % 2],false);
            _spawnedTunelsList.Add(spawnedObj);
            //SpawnObject(tunelObject, _spawnPoints[i % 2]);
        }
    }
    
    private void SpawnObject(GameObject toSpawn, Transform spawnPoint) // ak neni objekt null tak ho spawne do spawnpointu a nastav� mu parent kde sa spawnol
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

    private GameObject SpawnObjectGet(GameObject toSpawn)
    {
        Vector3 originalPosition = toSpawn.transform.position;
        Quaternion originalRotation = toSpawn.transform.rotation;
        GameObject spawnedObject = Instantiate(toSpawn, originalPosition, originalRotation);
        return spawnedObject;
    }
    private GameObject SpawnPOIGet(POI poi)
    {
        GameObject toSpawn;
        if (poi.ChanceToSpawn < RandomNumGen.Range(0f, 0.9f)) return null;
        else toSpawn = poi.Poi;
        Vector3 originalPosition = toSpawn.transform.position;
        Quaternion originalRotation = toSpawn.transform.rotation;
        GameObject spawnedObject = Instantiate(toSpawn, originalPosition, originalRotation);
        return spawnedObject;
    }
    /*
    private SpawnableGeneral InstantiateRandomSpawnable(List<SpawnableGeneral> list) // vyberie sa random SpawnableGeneral ktor� sa potom vyma�e z listu
    {
        ListItem<SpawnableGeneral> result = list.RemoveRandomItemFromList();
        SpawnableGeneral removedItem = result.Spawnable;
        _spawnablesList = result.SpawnablesList;
        return removedItem != null ? removedItem : null;
    }*/

    /*
    private GameObject InstantiateRandomPuzzleRoom()
    {
        int puzzleRoomsCount = _puzzleRooms.PuzzleRoomsList.Count;
        if (puzzleRoomsCount == 0) return null;
        int randomIndex = RandomNumGen.Range(0, puzzleRoomsCount);
        PuzzleRoom randomPuzzleRoom = _puzzleRooms.PuzzleRoomsList[randomIndex];
        Debug.Log("Chosen Room: " + randomIndex);
        if (randomPuzzleRoom != null) return randomPuzzleRoom.prefab.gameObject;
        else return null;
    */
}
