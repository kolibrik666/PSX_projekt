using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointPOIGetter : SpawnpointGetter
{
    public List<Transform> SpawnpointsList => _spawnpoints;
    private void OnEnable()
    {
        base.SetSpawnpointType(SpawnpointTypes.POI);
    }
}
