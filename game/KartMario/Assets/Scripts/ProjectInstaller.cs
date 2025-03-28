using Injecta;
using UnityEngine;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField]
    private Singleton singleton;

    public override void InstallBindings()
    {
        Container.BindInstance(singleton);
    }
}
