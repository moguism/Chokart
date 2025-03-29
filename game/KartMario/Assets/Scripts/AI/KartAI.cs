using UnityEngine;
using UnityEngine.AI;

public class KartAI : MonoBehaviour
{
    public Transform destination;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private SphereCollider sphereCollider;

    /*[SerializeField]
    private KartController kart;*/

    public float speed = 2000;

    private void Start()
    {
        destination = FindFirstObjectByType<FinishLine>().transform;
    }

    private void Update()
    {
        agent.destination = destination.position;
        sphereCollider.transform.position = agent.transform.position;
    }
}
