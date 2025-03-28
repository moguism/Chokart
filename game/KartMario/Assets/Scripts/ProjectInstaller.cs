using Injecta;
using UnityEngine;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField]
    private WebsocketSingleton singleton;

    public override void InstallBindings()
    {
        Container.BindInstance(singleton);
    }
}
