using Unity.Netcode;
using UnityEngine;

public class KartAI : NetworkBehaviour
{
    public Transform destination; // proximo trigger al que va a ir

    private int currentTriggerIndex = 0; // indice del trigeer por el que va

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
        if (kart != null)
        {
            kart.ai = this;
        }

        Debug.Log("el coche bot es ", kart);
    }

    private void Start()
    {
        UpdateDestination();
        Debug.Log("IA empieza con destino: " + destination.name);
    }

    private void Update()
    {
        if (!destination || !kart || !kart.enableAI) return;

        Vector3 localTarget = kart.transform.InverseTransformPoint(destination.position);
        HorizontalInput = Mathf.Clamp(localTarget.x, -1f, 1f) * steerSensitivity;

        MoveDirection = (localTarget.z > 0.5f) ? 1 : (localTarget.z < -0.5f ? -1 : 0);

        if (parent != null && parent.enableAI)
        {
            // Debug.Log("COCHE " + parent.GetComponentIndex() + "IA LE ENVIA LA DIRECCION" + HorizontalInput + " y " + MoveDirection); 
            parent.horizontalInput = HorizontalInput;
            parent.direction = MoveDirection;
        }
    }


    public void UpdateDestination()
    {
        if (MapTrigger.finishLine == null || MapTrigger.finishLine.triggers.Count == 0) return;

        currentTriggerIndex = (kart.lastTriggerIndex + 1) % MapTrigger.finishLine.triggers.Count;
        destination = MapTrigger.finishLine.triggers[currentTriggerIndex].transform;

        Debug.Log("IA actualiza destino a: " + destination.name);
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
