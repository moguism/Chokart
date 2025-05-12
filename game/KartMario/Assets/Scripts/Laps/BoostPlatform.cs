using UnityEngine;

public class BoostPlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        if (parent && parent.CompareTag("Kart"))
        {
            KartController kart = parent.GetComponentInChildren<KartController>();
            kart.Boost(true);
        }
    }
}
