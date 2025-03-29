using UnityEngine;

public class BotCollider : MonoBehaviour
{
    [SerializeField]
    public KartAI ai;

    private void OnTriggerEnter(Collider other)
    {
        var parent = other.gameObject.transform.parent;
        if (parent && parent.CompareTag("Kart"))
        {
            KartController otherKart = parent.GetComponentInChildren<KartController>();
            if (!otherKart.ai.Equals(ai))
            {
                ai.enemyKart = otherKart;
            }
        }
    }
}
