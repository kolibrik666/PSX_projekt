using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] GameStartData _gameStartData;
    [SerializeField] SpawnableConsumable _spawnableConsumableSO;
    [SerializeField] GameObject _spawnPOI;

    public override void InstallBindings()
    {
        Container.Bind<GameSetupData>().AsSingle().NonLazy();
        Container.Bind<GameStartData>().FromScriptableObject(_gameStartData).AsSingle();
        Container.BindFactory<SpawnPOI.InitData, SpawnPOI, SpawnPOI.Factory>().FromComponentInNewPrefab(_spawnPOI).AsSingle();
        Container.BindFactory<Consumable, Consumable.Factory>().FromComponentInNewPrefab(_spawnableConsumableSO.SpawnablePrefab);

        //Container.BindFactory<Consumable, Consumable.Factory>().FromScriptableObjectResource(_spawnableConsumableSO.SpawnablePrefab).NonLazy();

        /*Container.BindFactory<VehicleInstaller.Data, PlayerUnit, PlayerUnit.Factory>()
            .FromSubContainerResolve().ByNewContextPrefab<VehicleInstaller>(_playerUnitPrefab)
            .UnderTransform(_vehiclesParent).AsSingle();*/
    }
}