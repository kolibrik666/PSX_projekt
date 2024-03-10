using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointConsumableGetter : SpawnpointGetter
{
    public List<Transform> SpawnpointsList => _spawnpoints;
    private void OnEnable()
    {
        base.SetSpawnpointType(SpawnpointTypes.Consumable);
    }

}
