using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointRoomGetter : SpawnpointGetter
{
    public override void SetSpawnpointType(int index)
    {
        if (Enum.IsDefined(typeof(SpawnpointTypes), index)) _spawnpointTypes = (SpawnpointTypes)index;
        else Debug.LogError("Invalid SpawnpointTypes index.");
    }
}
