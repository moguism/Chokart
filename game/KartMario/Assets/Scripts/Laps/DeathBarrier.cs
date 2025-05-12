using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!LobbyManager.isHost)
        {
            return;
        }

        var parent = other.gameObject.transform.parent;
        if (parent && parent.CompareTag("Kart"))
        {
            KartController kart = parent.GetComponentInChildren<KartController>();
            DetectCollision detectCollision = FindFirstObjectByType<DetectCollision>();
            detectCollision.DisableAndDespawnKart(kart, kart.NetworkObjectId);
        }
    }
}
