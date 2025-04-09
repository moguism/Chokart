using Unity.Netcode;

public class BasicObject : NetworkBehaviour
{
    public ulong owner; // Para que un caparazón, por ejemplo, no pueda chocar contra el que lo lanzó
    public NetworkObject networkObject;

    // Este es el método que hay que hacerle override
    public void UseObject(){}

    [ServerRpc]
    public void DespawnOnTimeServerRpc()
    {
        FindAnyObjectByType<ObjectSpawner>().DespawnObjectServerRpc(this);
    }

}
