using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[System.Serializable]
public class ConsumableDictionaryItem
{
    public Consumable Key;
    public GameObject Value;
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
            if (item.Key.ConsumableType == consumable.ConsumableType)
            {
                item.Value.SetActive(true);
            }
            else Destroy(item.Value);
        }
    }
    public class Factory : PlaceholderFactory<InitData, SetConsumables> { }
    public record InitData
    {
        public Consumables Consumables;
    }
}
