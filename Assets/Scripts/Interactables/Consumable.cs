
using UnityEngine;
using Zenject;

public class Consumable : Interactable
{
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameStartData _gameStartData;
    [SerializeField] ConsumableTypes _consumableTypes;
    bool _isMagazine;
    private void OnEnable()
    {
        _isMagazine = _consumableTypes == ConsumableTypes.Magazine ? true : false;
    }
    public void AddValue(int val)
    {
        if (!_isMagazine) _gameSetupData.Saturation += val;
        else _gameSetupData.Sanity += val;
    }
    public override void OnFocus()
    {
        
    }

    public override void OnInteract()
    {
        int result = _consumableTypes switch
        {
            ConsumableTypes.Food => _gameStartData.FoodValue,
            ConsumableTypes.Magazine => _gameStartData.MagazineValue,
            ConsumableTypes.Beer => _gameStartData.BeerBalue,
            _ => 1
        };

        AddValue(result);
        Destroy(gameObject);
    }

    public override void OnLoseFocus()
    {

    }
    
}
