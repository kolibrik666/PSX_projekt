using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Zenject.CheatSheet;

public class GameInstaller : MonoInstaller
{
    [SerializeField] GameStartData _gameStartData;
    [SerializeField] SpawnableConsumable _spawnableConsumableSO;
    [SerializeField] GameObject _spawnPOI;
    [SerializeField] GameObject _setConsumables;

    public override void InstallBindings()
    {
        Container.Bind<GameSetupData>().AsSingle().NonLazy();
        Container.Bind<GameStartData>().FromScriptableObject(_gameStartData).AsSingle();
        Container.BindFactory<SpawnPOI.InitData, SpawnPOI, SpawnPOI.Factory>().FromComponentInNewPrefab(_spawnPOI).AsSingle();
        Container.BindFactory<Consumable, Consumable.Factory>().FromComponentInNewPrefab(_spawnableConsumableSO.SpawnablePrefab);
        Container.BindFactory<SetConsumables.InitData, SetConsumables, SetConsumables.Factory>().FromComponentInNewPrefab(_setConsumables);
    }
}