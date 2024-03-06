using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] GameStartData _gameStartData;
    public override void InstallBindings()
    {
        Container.Bind<GameSetupData>().AsSingle().NonLazy();
        Container.Bind<GameStartData>().FromScriptableObject(_gameStartData).AsSingle();
    }
}