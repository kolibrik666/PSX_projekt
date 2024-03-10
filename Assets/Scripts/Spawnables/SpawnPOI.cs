using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPOI : MonoBehaviour
{
    Consumables _consumables;
    List<GameObject> _spawnedConsumablesObjects = new();

    List<GameObject> _spawnedPOIsObjects = new();
    [Inject] Consumable.Factory _consumable = default;

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
                //var consumable = _consumables.ConsumablesList.GetRandomItemFromList().SpawnablePrefab;
                var selectedObj = _consumable.Create();
                selectedObj.gameObject.transform.SetParent(obj.transform, false);
            });
        });
    }

    public class Factory : PlaceholderFactory<InitData, SpawnPOI> { }

    public record InitData
    {
        public List<GameObject> SpawnedPOIsObjects;
        public Consumables Consumables;
    }
}
