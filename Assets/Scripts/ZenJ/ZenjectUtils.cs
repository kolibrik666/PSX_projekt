using Zenject;

public class ZenjectUtils : IInitializable
{
    DiContainer _container;

    public static ZenjectUtils Instance { get; private set; }
    public DiContainer Container { set => _container = value; }

    public void Initialize()
    {
        Instance ??= this;
        _container = ProjectContext.Instance.Container;
    }

    public void Inject(object obj)
    {
        var type = obj.GetType();
        if (type.IsGenericType) UnityEngine.Debug.LogError($"Trying to inject generic type {type}!");
        if (_container == null)
        {
            UnityEngine.Debug.LogError($"Container is null!");
            return;
        }
        _container.Inject(obj);
    }
}
