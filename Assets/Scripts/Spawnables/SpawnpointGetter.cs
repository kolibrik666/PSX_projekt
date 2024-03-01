using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointGetter : MonoBehaviour
{
    [SerializeField] Transform _spawnpoint;
    SpawnpointTypes _spawnpointTypes;
    public Transform Spawnpoint => _spawnpoint;
    public SpawnpointTypes SpawnpointType => _spawnpointTypes;
    public void SetSpawnpointType(SpawnpointTypes newSpawnpointType)
    {
        _spawnpointTypes = newSpawnpointType;
    }
    public void SetSpawnpointType(int index)
    {
        if (Enum.IsDefined(typeof(SpawnpointTypes), index)) _spawnpointTypes = (SpawnpointTypes)index;
        else Debug.LogError("Invalid SpawnpointTypes index.");
    }
}
