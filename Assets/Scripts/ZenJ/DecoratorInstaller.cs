using UnityEngine;
using Zenject;

public class DecoratorInstaller : MonoInstaller
{
    [SerializeField] Transform _canvasParent = default;

    [SerializeField] GameObject _crosshair;
    public override void InstallBindings()
    {
        Container.BindFactory<Crosshair, Crosshair.Factory>()
                 .FromComponentInNewPrefab(_crosshair).UnderTransform(_canvasParent).AsSingle();
    }
}
