using Unity.Netcode;

public class BasicObject : NetworkBehaviour
{
    public float owner; // Para que un caparaz�n, por ejemplo, no pueda chocar contra el que lo lanz�
    public NetworkObject networkObject;

    // Este es el m�todo que hay que hacerle override
    public void UseObject(){}
  
}
