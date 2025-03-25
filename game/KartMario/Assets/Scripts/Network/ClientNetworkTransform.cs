using Unity.Netcode.Components;

// Para sincronizar la posición
public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
