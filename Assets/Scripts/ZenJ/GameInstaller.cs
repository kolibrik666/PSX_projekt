using UnityEngine;
using Zenject;


public class GameInstaller : MonoInstaller
{
    [SerializeField] GameStartData _gameStartData;
    [SerializeField] GameObject _spawnPOI;
    [SerializeField] GameObject _setConsumables;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] CommonSounds _commonSounds;
    [SerializeField] AudioPlayer _audioPlayerPrefab;

    [SerializeField] SpawnableGeneral _setupSpotlightPuzzle;

    [Header("Puzzles")]
    [SerializeField] GameObject _trezorPuzzleRoom;
    [SerializeField] GameObject _spotlightPuzzleRoom;

    public override void InstallBindings()
    {
        Container.Bind<GameSetupData>().AsSingle().NonLazy();
        Container.Bind<GameRunData>().AsSingle().NonLazy();
        Container.Bind<GameStartData>().FromScriptableObject(_gameStartData).AsSingle();
        Container.BindFactory<SpawnPOI.InitData, SpawnPOI, SpawnPOI.Factory>().FromComponentInNewPrefab(_spawnPOI).AsSingle();

        Container.BindFactory<SetConsumables.InitData, SetConsumables, SetConsumables.Factory>().FromComponentInNewPrefab(_setConsumables);
        Container.BindInterfacesAndSelfTo<AudioManager>().FromScriptableObject(_audioManager).AsSingle();
        Container.BindInterfacesAndSelfTo<CommonSounds>().FromScriptableObject(_commonSounds).AsSingle();
        Container.BindMemoryPool<AudioPlayer, AudioPlayer.Pool>().WithInitialSize(8)
           .FromComponentInNewPrefab(_audioPlayerPrefab).AsSingle();
        Container.BindInterfacesAndSelfTo<ZenjectUtils>().AsSingle();
        Container.BindInterfacesAndSelfTo<Serializer>().AsSingle().NonLazy();

        Container.BindFactory<SetupPuzzle, SetupPuzzle.Factory>();

        Container.BindFactory<TrezorPuzzleRoom, TrezorPuzzleRoom.Factory>().FromComponentInNewPrefab(_trezorPuzzleRoom);
        Container.BindFactory<SpotlightPuzzleRoom, SpotlightPuzzleRoom.Factory>().FromComponentInNewPrefab(_spotlightPuzzleRoom);

    }


}