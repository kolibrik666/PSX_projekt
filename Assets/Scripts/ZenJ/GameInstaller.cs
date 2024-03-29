using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Zenject;
using static Zenject.CheatSheet;

public class GameInstaller : MonoInstaller
{
    [SerializeField] GameStartData _gameStartData;
    [SerializeField] SpawnableConsumable _spawnableConsumableSO;
    [SerializeField] GameObject _spawnPOI;
    [SerializeField] GameObject _setConsumables;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] CommonSounds _commonSounds;
    [SerializeField] AudioPlayer _audioPlayerPrefab;
    public override void InstallBindings()
    {
        Container.Bind<GameSetupData>().AsSingle().NonLazy();
        Container.Bind<GameRunData>().AsSingle().NonLazy();
        Container.Bind<GameStartData>().FromScriptableObject(_gameStartData).AsSingle();
        Container.BindFactory<SpawnPOI.InitData, SpawnPOI, SpawnPOI.Factory>().FromComponentInNewPrefab(_spawnPOI).AsSingle();
        Container.BindFactory<Consumable, Consumable.Factory>().FromComponentInNewPrefab(_spawnableConsumableSO.SpawnablePrefab);
        Container.BindFactory<SetConsumables.InitData, SetConsumables, SetConsumables.Factory>().FromComponentInNewPrefab(_setConsumables);
        Container.BindInterfacesAndSelfTo<AudioManager>().FromScriptableObject(_audioManager).AsSingle();
        Container.BindInterfacesAndSelfTo<CommonSounds>().FromScriptableObject(_commonSounds).AsSingle();
        Container.BindMemoryPool<AudioPlayer, AudioPlayer.Pool>().WithInitialSize(5)
           .FromComponentInNewPrefab(_audioPlayerPrefab).AsSingle();
        Container.BindInterfacesAndSelfTo<ZenjectUtils>().AsSingle();
        Container.BindInterfacesAndSelfTo<Serializer>().AsSingle().NonLazy();
    }
}