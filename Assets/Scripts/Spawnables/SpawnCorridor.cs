using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCorridor : MonoBehaviour
{
    /*[SerializeField] SpawnableGeneral _metalDoor;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] ScriptableObject _spawnObject;

    TunelPreRooms _preTunelRooms;

    List<SpawnableGeneral> _spawnablesList;
    List<SpawnableGeneral> _spawnedCorridorList;

    private void OnEnable()
    {
        //SetupCorridor();
    }

    private void SetupCorridor(int spawnablesCount, Transform spawnPoint, List<SpawnableGeneral> tunelPreRoomsList)
    {
        _spawnablesList = tunelPreRoomsList;
        _spawnedCorridorList = new List<SpawnableGeneral>();

        if (_spawnablesList == null || spawnablesCount == 0) return;
        var objectGeneral = _spawnablesList.RemoveRandomItem();


        foreach (var spawnable in _spawnedCorridorList) //zlÈ sa spawnuju na s˙radniciach, proste pre kaûd˙ jednu zvl·öù spawn? a porovn·ö? Ëi uû existuje taka
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
            Vector3 originalPosition = tunelObject.transform.position;
            Quaternion originalRotation = tunelObject.transform.rotation;
            GameObject spawnedTunnel = Instantiate(tunelObject, originalPosition, originalRotation);

            spawnedTunnel.transform.SetParent(spawnPoint, false);
        }
        else Debug.LogError("Tunel is not possible to load/read!");
    }*/
}
