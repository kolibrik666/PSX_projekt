using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPOI : MonoBehaviour
{
    Consumables _consumables;

    List<GameObject> _spawnedPOIsObjects = new();
    [Inject] SetConsumables.Factory _setConsumables = default;

    [Inject]
    public void Construct(InitData initData)
    {
        _spawnedPOIsObjects = initData.SpawnedPOIsObjects;
        _consumables = initData.Consumables;
    }
    private void OnEnable()
    {
        _spawnedPOIsObjects.ForEach(obj =>
        {
            var spawnpointConsumableList = obj.GetComponent<SpawnpointConsumableGetter>().SpawnpointsList;
            spawnpointConsumableList.ForEach(obj =>
            {
                var selectedObj = _setConsumables.Create(new()
                {
                    Consumables = _consumables,
                });
                selectedObj.gameObject.transform.SetParent(obj.transform, false);
            });
        });
        Destroy(gameObject);
    }

    public class Factory : PlaceholderFactory<InitData, SpawnPOI> { }

    public record InitData
    {
        public List<GameObject> SpawnedPOIsObjects;
        public Consumables Consumables;
    }
}
