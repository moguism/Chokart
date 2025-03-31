using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class KartAI : NetworkBehaviour
{
    public Transform destination;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private SphereCollider sphereCollider;

    [SerializeField]
    public KartController parent;

    public KartController enemyKart;

    /*[SerializeField]
    private KartController kart;*/

    public float speed = 2000;

    private void Start()
    {
        destination = FindFirstObjectByType<FinishLine>().transform;
    }

    // Básicamente el comportamiento es que en cuanto algún coche entra en su rango de visión, le persigue hasta que acaba con él, usando objetos también
    // Una vez acaba con él, va hacia el siguiente trigger del mapa
    private void LateUpdate()
    {
        if(!IsOwner)
        {
            return;
        }

        parent.sphere.isKinematic = true;

        if (enemyKart != null)
        {
            if (parent.currentObject != "")
            {
                parent.SpawnObject();
            }

            destination = enemyKart.transform;
        }

        agent.destination = destination.position;
        parent.sphere.transform.position = agent.transform.position;

        parent.sphere.isKinematic = false;
    }
}
