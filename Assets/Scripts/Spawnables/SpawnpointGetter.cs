using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointGetter : MonoBehaviour
{
    [SerializeField] protected List <Transform> _spawnpoints;
    protected SpawnpointTypes _spawnpointTypes;
    public Transform Spawnpoint => _spawnpoints.GetRandomItemFromList();
    public SpawnpointTypes SpawnpointType => _spawnpointTypes;
    public virtual void SetSpawnpointType(SpawnpointTypes newSpawnpointType)
    {
        _spawnpointTypes = newSpawnpointType;
    }
    public virtual void SetSpawnpointType(int index)
    {

    }
}
