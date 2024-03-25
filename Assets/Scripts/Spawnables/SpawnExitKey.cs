using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExitKey : MonoBehaviour
{
    [SerializeField] List<Transform> _keySpawnpoints = new();
    [SerializeField] SpawnableConsumable _key;
    GameObject _keyExit;
    public GameObject KeyExit => _keyExit;
    private void OnEnable()
    {
        Transform randomSpawnPoint = _keySpawnpoints[RandomNumGen.Range(0, _keySpawnpoints.Count)];
        _keyExit = Instantiate(_key.SpawnablePrefab, _key.SpawnablePrefab.transform.position, _key.SpawnablePrefab.transform.rotation);
        _keyExit.transform.SetParent(randomSpawnPoint.transform, false);

    }
}
