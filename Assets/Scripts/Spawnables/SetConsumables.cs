using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class ConsumableDictionaryItem
{
    public Consumable Script;
    public GameObject Prefab;
}
public class SetConsumables : MonoBehaviour
{
    [SerializeField] List<ConsumableDictionaryItem> _consumablesList = new();
    Consumables _consumables;
    
    [Inject]
    public void Construct(InitData initData)
    {
        _consumables = initData.Consumables;
    }
    private void OnEnable()
    {
        var consumable = _consumables.ConsumablesList.GetRandomItemFromList();  
        foreach (var item in _consumablesList)
        {
            if (item.Script.ConsumableType == consumable.ConsumableType)
            {
                item.Prefab.SetActive(true);
            }
            else Destroy(item.Prefab);
        }
    }
    public class Factory : PlaceholderFactory<InitData, SetConsumables> { }
    public record InitData
    {
        public Consumables Consumables;
    }
}
