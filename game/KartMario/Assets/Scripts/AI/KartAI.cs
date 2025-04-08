using Unity.Netcode;
using UnityEngine;

public class KartAI : NetworkBehaviour
{
    public Transform destination; // proximo trigger al que va a ir

    public float steerSensitivity = 1.0f;

    public float HorizontalInput { get; private set; }
    public int MoveDirection { get; private set; }

    /*[SerializeField]
    private NavMeshAgent agent;*/

    [SerializeField]
    private SphereCollider sphereCollider;

    [SerializeField]
    public KartController parent;

    public KartController enemyKart;

    /*[SerializeField]*/
    private KartController kart; // el coche

    public float speed = 2000;

    private void Awake()
    {
        kart = GetComponent<KartController>();
        Debug.Log("el coche es ", kart); // esto lo pilla bien
    }

    private void Start()
    {
        destination = FindFirstObjectByType<FinishLine>().transform;
        Debug.Log("el destino de la ia es ", destination); // lo pilla bien
    }

    private void Update()
    {
        if (!destination || !kart) return;

        Vector3 localTarget = kart.transform.InverseTransformPoint(destination.position);
        HorizontalInput = Mathf.Clamp(localTarget.x, -1f, 1f) * steerSensitivity;

        MoveDirection = (localTarget.z > 0.5f) ? 1 : (localTarget.z < -0.5f ? -1 : 0);

        if (parent != null && parent.enableAI) // si quitas lo de enableIA el jugador con el que hostees FUNCIONA COMO BOT y el bot que pongas NO :(((
        {
            Debug.Log("COCHE " + parent.GetComponentIndex() + "IA LE ENVIA LA DIRECCION" + HorizontalInput + " y " + MoveDirection); // entra bien
            parent.horizontalInput = HorizontalInput;
            parent.direction = MoveDirection;
        }
    }

    // Básicamente el comportamiento es que en cuanto algún coche entra en su rango de visión, le persigue hasta que acaba con él, usando objetos también
    // Una vez acaba con él, va hacia el siguiente trigger del mapa
    private void LateUpdate()
    {
        if (!IsOwner)
        {
            Debug.LogError("NO ES DUEÑO DE SU PROPIO COCHE");
            return;
        }
        /*
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

        parent.sphere.isKinematic = false;*/
    }
}
