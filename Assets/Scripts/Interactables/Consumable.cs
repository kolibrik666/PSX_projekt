
using System;
using UnityEditor;
using UnityEngine;
using Zenject;

public class Consumable : Interactable
{
    [Inject] GameStartData _gameStartData;
    [Inject] GameRunData _gameRunData;
    [Inject] AudioManager _audioManager;
    [Inject] CommonSounds _commonSounds;
    [SerializeField] ConsumableTypes _consumableTypes;

    public ConsumableTypes ConsumableType => _consumableTypes;
    public static event Action OnValueChanged;

    public void AddValue(int val)
    {
        if (_consumableTypes != ConsumableTypes.Magazine) _gameRunData.Saturation += val;
        else _gameRunData.Sanity += val;
        OnValueChanged?.Invoke();
    }
    public override void OnFocus()
    {

    }

    public override void OnInteract()
    {
        switch (_consumableTypes)
        {
            case ConsumableTypes.Beer:
                _audioManager.PlayOneShot(_commonSounds.SwallowFood);
                AddValue(_gameStartData.BeerBalue);
                break;
            case ConsumableTypes.Magazine:
                _audioManager.PlayOneShot(_commonSounds.TakePaper);
                AddValue(_gameStartData.MagazineValue);
                break;
            case ConsumableTypes.Key:
                break;
            default:
                AddValue(_gameStartData.FoodValue);
                _audioManager.PlayOneShot(_commonSounds.SwallowFood);
                break;
        }

        Destroy(gameObject);
    }

    public override void OnLoseFocus()
    {

    }
    public class Factory : PlaceholderFactory<Consumable> { }


}
