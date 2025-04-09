using Unity.Netcode;
using UnityEngine;

public class KartAI : NetworkBehaviour
{
    public Transform destination; // proximo trigger al que va a ir

    private int currentTriggerIndex = 0; // indice del trigeer por el que va

    public float steerSensitivity = 1.0f;

    private bool isDrifting = false;
    private float driftTimer; // lo que van a durar los derrapes 

    private bool botDrift; // esto marca si el bot va a derrapar o no durante la partida, 

    public float HorizontalInput { get; private set; }
    public int MoveDirection { get; private set; }

    /*[SerializeField]
    private NavMeshAgent agent;*/

    [SerializeField]
    private SphereCollider sphereCollider;

    /* [SerializeField]
     public KartController parent;*/

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

        botDrift = Random.value < 0.5f;

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
        /*
        // derrapes automaticos en curvas pronunciadas
        float angle = Vector3.Angle(kart.transform.forward, (destination.position - kart.transform.position).normalized);

        // 50% de probabilidad de que un bot que derrape, para que no todos lo hagan igual
        if (!isDrifting && angle > 20f && Mathf.Abs(HorizontalInput) >= 1f && Random.value < 0.5f && botDrift)
        {
            isDrifting = true;
            driftTimer = Random.Range(0.5f, 3f);
            kart.jumping = true;
        }

        // derrapa x tiempo
        if (isDrifting)
        {
            driftTimer -= Time.deltaTime;
            kart.jumping = true;

            if (driftTimer <= 0)
            {
                isDrifting = false;
                kart.jumping = false;
            }
        }
        else
        {
            kart.jumping = false;
        }*/


        Debug.Log("COCHE " + kart.GetComponentIndex() + " IA LE ENVIA LA DIRECCION" + HorizontalInput + " y " + MoveDirection);
        kart.horizontalInput = HorizontalInput;
        kart.direction = MoveDirection;

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
