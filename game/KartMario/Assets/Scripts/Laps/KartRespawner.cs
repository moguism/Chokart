using FMODUnity;
using UnityEngine;

public class KartRespawner : MonoBehaviour
{
    public KartController kart;
    public float fallThreshold = -100f;

    private void Awake()
    {
        kart = GetComponent<KartController>();
        Debug.Log("El reespawn es de " + kart.GetComponentIndex());
    }

    private void Update()
    {
        if (kart.transform.To3DAttributes().position.y < fallThreshold)
        {
            Debug.Log("Reespawneandooooo");
            RespawnAtNextTrigger();
        }
    }

    private void RespawnAtNextTrigger()
    {
        // Calcula el siguiente trigger
        int nextIndex = (kart.lastTriggerIndex + 1) % MapTrigger.finishLine.triggers.Count;
        Transform nextTriggerTransform = MapTrigger.finishLine.triggers[nextIndex].transform;

        Transform root = kart.transform.root;
        root.position = nextTriggerTransform.position;
        root.rotation = nextTriggerTransform.rotation;


        Debug.Log("Listo para reaparecer en " + nextTriggerTransform.name);


        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        kart.canMove = true;

        Debug.Log($"[{kart.name}] reaparecio en el trigger {nextIndex} ({nextTriggerTransform.name})");
    }
}
