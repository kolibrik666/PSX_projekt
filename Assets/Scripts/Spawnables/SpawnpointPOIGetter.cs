using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointPOIGetter : SpawnpointGetter
{
    public List<Transform> SpawnpointsList => _spawnpoints;
    public POITypes POIType;
    // typ ake POI TO MA BYT AKE SA MA SPAWNUT
    private void OnEnable()
    {
        base.SetSpawnpointType(SpawnpointTypes.POI);
    }
}
