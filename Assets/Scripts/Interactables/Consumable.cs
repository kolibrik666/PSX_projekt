
using System;
using UnityEditor;
using UnityEngine;
using Zenject;

public class Consumable : Interactable
{
    [Inject] GameStartData _gameStartData;
    [Inject] GameRunData _gameRunData;

    [SerializeField] ConsumableTypes _consumableTypes;

    bool _isMagazine;
    public ConsumableTypes ConsumableType => _consumableTypes;
    public static event Action OnValueChanged;

    private void OnEnable()
    {
        _isMagazine = _consumableTypes == ConsumableTypes.Magazine ? true : false;
    }
    public void AddValue(int val)
    {
        if (!_isMagazine) _gameRunData.Saturation += val;
        else _gameRunData.Sanity += val;
        OnValueChanged?.Invoke();
    }
    public override void OnFocus()
    {
    }

    public override void OnInteract()
    {
        int result = _consumableTypes switch
        {
            ConsumableTypes.Magazine => _gameStartData.MagazineValue,
            ConsumableTypes.Beer => _gameStartData.BeerBalue,
            _ => _gameStartData.FoodValue
        };

        AddValue(result);
        Destroy(gameObject);
    }

    public override void OnLoseFocus()
    {

    }
    public class Factory : PlaceholderFactory<Consumable> { }


}
