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

    public KartController enemyKart;

    /*[SerializeField]*/
    private KartController kart; // el coche

    // atascos
    float stuckCheckInterval = 2f;
    float stuckTimer = 0f;
    Vector3 lastPosition;
    bool isStuck = false;

    private void Awake()
    {
        kart = GetComponent<KartController>();
        if (kart != null)
        {
            kart.ai = this;
        }

        botDrift = Random.value < 0.5f;
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
        float distance = localTarget.magnitude;
        HorizontalInput = Mathf.Clamp(localTarget.x / distance, -1f, 1f) * steerSensitivity;

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

        // que no se quede yendo marcha atras sino que gire
        /*float angle = Vector3.Angle(kart.transform.forward, (destination.position - kart.transform.position).normalized);
        if (MoveDirection == -1 && angle > 90f)
        {
            HorizontalInput = -HorizontalInput;
            MoveDirection = 1;
        }*/

        Debug.Log("COCHE " + kart.GetComponentIndex() + " LE ENVIA LA DIRECCION" + HorizontalInput + " y " + MoveDirection);
        Debug.Log("COCHE " + kart + " va a " + destination);
        kart.horizontalInput = HorizontalInput;
        kart.direction = MoveDirection;

    }

    /*private void OnDrawGizmos()
    {
        if (destination != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, destination.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(lastPosition, 0.2f);

            if (isStuck)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, Vector3.one);
            }
        }
    }*/




    public void UpdateDestination()
    {
        if (MapTrigger.finishLine == null || MapTrigger.finishLine.triggers.Count == 0) return;

        currentTriggerIndex = (kart.lastTriggerIndex + 1) % MapTrigger.finishLine.triggers.Count;
        destination = MapTrigger.finishLine.triggers[currentTriggerIndex].transform;

        Debug.Log("IA actualiza destino a: " + destination.name);
    }

    // B�sicamente el comportamiento es que en cuanto alg�n coche entra en su rango de visi�n, le persigue hasta que acaba con �l, usando objetos tambi�n
    // Una vez acaba con �l, va hacia el siguiente trigger del mapa
    private void LateUpdate()
    {
        if (!IsOwner)
        {
            //Debug.Log("NO ES DUE�O DE SU PROPIO COCHE");
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
