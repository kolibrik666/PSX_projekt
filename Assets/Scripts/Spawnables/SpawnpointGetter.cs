using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointGetter : MonoBehaviour
{
    [SerializeField] Transform _spawnpoint;
    public Transform Spawnpoint => _spawnpoint;
}
