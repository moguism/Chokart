using Unity.Netcode.Components;

// Para sincronizar la posici�n
public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
