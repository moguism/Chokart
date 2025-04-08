using Injecta;
using UnityEngine;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField]
    private WebsocketSingleton websocketSingleton;

    [SerializeField]
    private HealthChanger healthChanger;

    public override void InstallBindings()
    {
        Container.BindInstance(websocketSingleton);
        Container.BindInstance(healthChanger);
    }
}
