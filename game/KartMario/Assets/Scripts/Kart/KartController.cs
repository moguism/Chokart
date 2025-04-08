using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class KartController : BasicPlayer
{
    // Para los efectos (el bloom sobre todo vaya)
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    public List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    public List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    public float speed, currentSpeed;
    float rotate, currentRotate;
    int driftDirection;
    float driftPower;
    int driftMode = 0;
    bool first, second, third;
    Color c;

    [Header("Bools")]
    public bool drifting;

    [Header("Parameters")]

    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;

    [Header("Model Parts")]

    public Transform frontWheels;
    public Transform backWheels;
    public Transform steeringWheel;

    [Header("Particles")]
    public Transform wheelParticles;
    public Transform flashParticles;
    public Color[] turboColors;

    // Para el giroscopio
    public bool isMobile;
    public float horizontalInput;

    public int direction = 0;
    public bool jumping = false;
    public bool isGrounded = true;

    // Para las posiciones
    public int totalLaps = 0;
    public int position = 0;
    public bool passedThroughFinishLine = false;
    public List<int> triggers = new List<int>();
    public int lastTriggerIndex;
    public float distanceToNextTrigger;
    public TMP_Text positionText;
    public Vector3 currentPosition;
    private PositionManager positionManager;

    // Objetos
    public string currentObject;
    private ObjectSpawner objectSpawner;
    public bool canBeHurt = true;

    // UI
    public TMP_Text healthText;

    // AI
    public bool enableAI = false;
    public KartAI ai;

    // Selección
    [SerializeField]
    private int kartIndex;
    private GameStarter starter;

    // Controles
    private InputSystem_Actions playerControls;
    private float jumpValueLastFrame;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (enableAI)
            {
                ai.enabled = true;
                return;
            }

            if (WebsocketSingleton.kartModelIndex != -1 && kartIndex != WebsocketSingleton.kartModelIndex)
            {
                return;
            }

            CinemachineVirtualCamera camera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            camera.Follow = gameObject.transform;
            camera.LookAt = gameObject.transform;

            positionText = GameObject.Find("PositionValue").GetComponent<TMP_Text>();

            Speedometer speedometer = FindFirstObjectByType<Speedometer>();
            speedometer.kart = this;

            healthText = GameObject.Find("HealthText").GetComponent<TMP_Text>();

            // cronometro en marcha
            FindFirstObjectByType<Chronometer>().StartTimer();
        }
    }

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        InformServerKartCreatedServerRpc(NetworkObjectId);

        // Si me han asignado un modelo que no es
        if (WebsocketSingleton.kartModelIndex != -1 && kartIndex != WebsocketSingleton.kartModelIndex)
        {
            InformServerAboutCharacterChangeServerRpc(NetworkObjectId, WebsocketSingleton.kartModelIndex, OwnerClientId, transform.position);
            return;
        }

        playerControls = new InputSystem_Actions();
        playerControls.Enable();

        var positionManager = GameObject.Find("Triggers");
        positionManager.GetComponent<PositionManager>().karts.Add(this);

        isMobile = Application.isMobilePlatform;
        if (isMobile)
        {
            Input.gyro.enabled = true;
            //Screen.orientation = ScreenOrientation.LandscapeLeft; // Para rotar la pantalla
        }
        else
        {
            Destroy(GameObject.Find("Buttons"));
        }

        postVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProfile = postVolume.profile;

        // Para asignar los distintos colores de las partículas
        for (int i = 0; i < wheelParticles.GetChild(0).childCount; i++)
        {
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < wheelParticles.GetChild(1).childCount; i++)
        {
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }

        foreach (ParticleSystem p in flashParticles.GetComponentsInChildren<ParticleSystem>())
        {
            secondaryParticles.Add(p);
        }

        objectSpawner = FindFirstObjectByType<ObjectSpawner>();
    }


    // PARA PODER MANDARLE UN OBJETO HABRÍA QUE SERIALIZAR
    [ServerRpc]
    void InformServerKartCreatedServerRpc(ulong kartId, ServerRpcParams rpcParams = default)
    {
        if (positionManager == null)
        {
            positionManager = GameObject.Find("Triggers").GetComponent<PositionManager>();
        }

        // El servidor agrega el kart a la lista
        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart == null)
        {
            print("Agregando");
            positionManager.karts.Add(this);
        }
    }

    [ServerRpc]
    void InformServerKartStatusServerRpc(ulong kartId, Vector3 currentPositionToUpdate, ServerRpcParams rpcParams = default)
    {
        if (positionManager == null)
        {
            positionManager = GameObject.Find("Triggers").GetComponent<PositionManager>();
        }

        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            print("ME HA LLEGADO MENSAJE DEL COCHE " + NetworkObjectId);
            kart.currentPosition = currentPositionToUpdate;
        }
    }

    [ServerRpc]
    void InformServerAboutCharacterChangeServerRpc(ulong kartId, int desiredIndex, ulong ownerId, Vector3 position, ServerRpcParams rpcParams = default)
    {
        if (starter == null)
        {
            starter = FindFirstObjectByType<GameStarter>();
        }

        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.GetComponentInParent<NetworkObject>().Despawn(true);

        //NetworkManager.Singleton.NetworkConfig.PlayerPrefab = starter.PossiblePrefabs.ElementAt(desiredIndex);

        GameObject prefab = starter.PossiblePrefabs.ElementAt(desiredIndex);
        GameObject gameObject = Instantiate(prefab, position, Quaternion.identity);

        gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            float time = Time.timeScale == 1 ? .2f : 1;
            Time.timeScale = time;
        }*/

        if (!IsOwner)
        {
            return;
        }

        if (isMobile)
        {
            float gyroGravityX = Input.gyro.gravity.x;
            horizontalInput = Mathf.Clamp(gyroGravityX * 2f, -1f, 1f);
        }
        else
        {
            horizontalInput = playerControls.Player.Move.ReadValue<Vector2>().x;
            //horizontalInput = Input.GetAxis("Horizontal");
        }

        healthText.text = "Salud: \n" + health;

        // La colisión es la que se mueve y nosotros la seguimos (sinceramente npi de por qué todo dios lo hace así)
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
        currentPosition = transform.position;

        if (enableAI)
        {
            InformServerKartStatusServerRpc(NetworkObjectId, currentPosition);
            return;
        }

        // Moverse palante (en el vídeo lo del else no viene pero es que si no es muy cutre)
        if (direction == 1 || playerControls.Player.Fire1.ReadValue<float>() == 1)
        {
            speed = acceleration;
        }
        else if (direction == -1 || playerControls.Player.Fire2.ReadValue<float>() == 1)
        {
            speed = -acceleration;
        }
        else
        {
            speed = 0;
        }

        // Para girar el modelo a la izquierda o la derecha
        if (horizontalInput != 0 && speed != 0)
        {
            int dir = horizontalInput > 0 ? 1 : -1;
            float amount = Mathf.Abs(horizontalInput);
            Steer(dir, amount);
        }

        float jumpValue = playerControls.Player.Jump.ReadValue<float>();

        // AY MI MADRE EL DERRAPE
        if ((jumpValue == 1 && !drifting && jumpValueLastFrame == 0) || (jumping && !drifting))
        {

            print("SPEED: " + speed);
            print("Horizontal: " + horizontalInput);

            // En el tutorial no viene, pero yo quiero que pueda dar saltitos :(
            if (horizontalInput != 0 && speed != 0)
            {
                drifting = true;
                driftDirection = horizontalInput > 0 ? 1 : -1;

                foreach (ParticleSystem p in primaryParticles)
                {
                    var main = p.main;
                    main.startColor = Color.clear;
                    p.Play();
                }
            }

            if (!isMobile)
            {
                kartModel.parent.DOComplete();
                kartModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);
            }

        }

        if (drifting)
        {
            // El pavo del vídeo quería tener un rango entre 0 y 2 para controlar la fuerza del derrape
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(horizontalInput, -1, 1, 0, 2) : ExtensionMethods.Remap(horizontalInput, -1, 1, 2, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(horizontalInput, -1, 1, .2f, 1) : ExtensionMethods.Remap(horizontalInput, -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl;

            ColorDrift();
        }

        // Solución un poquito rata pero bueno xD
        if ((isMobile && !jumping && drifting) || (!isMobile && jumpValue == 0 && drifting))
        {
            print("HOLA: " + isMobile);
            Boost();
        }

        // Para poder moverse y rotar, idk
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        // Para rotar el modelo con y sin derrape (qué coño es un Quaternion 😭)
        if (!drifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + (horizontalInput * 15), kartModel.localEulerAngles.z), .2f);
        }
        else
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(horizontalInput, -1, 1, .5f, 2) : ExtensionMethods.Remap(horizontalInput, -1, 1, 2, .5f);

            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
        }

        // Pongo esto en un try-catch porque para testing he puesto un modelo sin nada de esto
        try
        {
            // Lo mismo pero con las ruedas y el volante
            frontWheels.localEulerAngles = new Vector3(0, (horizontalInput * 15), frontWheels.localEulerAngles.z);
            frontWheels.localEulerAngles += new Vector3(0, 0, sphere.linearVelocity.magnitude / 2);
            backWheels.localEulerAngles += new Vector3(0, 0, sphere.linearVelocity.magnitude / 2);

            steeringWheel.localEulerAngles = new Vector3(-25, 90, ((horizontalInput * 45)));
            //print("Soy el coche " + NetworkObjectId + " y estoy en " + currentPosition);
        }
        catch { }

        if (playerControls.Player.Fire3.ReadValue<float>() == 1)
        {
            if (currentObject != "")
            {
                SpawnObject();
            }
        }

        InformServerKartStatusServerRpc(NetworkObjectId, currentPosition);

        jumpValueLastFrame = jumpValue;
    }

    public void SpawnObject()
    {
        print("Spawneando...");

        if (IsOwner)
        {
            SpawnObjectServerRpc(currentObject, currentPosition, transform.TransformDirection(Vector3.forward), NetworkObjectId);
        }

        currentObject = "";
    }

    [ServerRpc]
    private void SpawnObjectServerRpc(string currentObject, Vector3 currentPosition, Vector3 destination, ulong kartId)
    {
        if(objectSpawner == null)
        {
            objectSpawner = FindFirstObjectByType<ObjectSpawner>();
        }

        objectSpawner.SpawnObject(currentObject, currentPosition, destination, kartId);
    }

    // FixedUpdate es como el _physics_process de Godot (se ejecuta cada cierto tiempo, siempre el mismo)
    // Es la esfera la que hace todo
    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        // Aceleración, gravedad y rotación, respectivamente
        if (!drifting)
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        // Para que si ha cochado con algo gire un poco el kart
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);

    }

    public void Boost()
    {
        drifting = false;

        if (driftMode > 0)
        {
            DOVirtual.Float(currentSpeed * 3, currentSpeed, 1.5f * driftMode, Speed); // Para aumentar la velocidad
            DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount)); // Dios como me encanta el bloom xD

            try
            {
                kartModel.Find("Tube001").GetComponentInChildren<ParticleSystem>().Play(); // Tubo de escape (contaminación :c)
                kartModel.Find("Tube002").GetComponentInChildren<ParticleSystem>().Play();
            }
            catch { }
        }

        // Una vez que ha hecho toda la pesca pone todo a sus valores por defecto
        driftPower = 0;
        driftMode = 0;
        first = false; second = false; third = false;

        foreach (ParticleSystem p in primaryParticles)
        {
            var main = p.main;
            main.startColor = Color.clear;
            p.Stop();
        }

        kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);

    }

    // Para controlar cuánto tiene que girar en función de la "fuerza" del input
    public void Steer(int direction, float amount)
    {
        if (speed < 0)
        {
            direction *= -1 ;
        }
        rotate = (steering * direction) * amount;
    }

    // Para definir el color de las partículas
    public void ColorDrift()
    {
        if (!first)
            c = Color.clear;

        if (driftPower > 50 && driftPower < 100 - 1 && !first)
        {
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
        }

        if (driftPower > 100 && driftPower < 150 - 1 && !second)
        {
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);
        }

        if (driftPower > 150 && !third)
        {
            third = true;
            c = turboColors[2];
            driftMode = 3;

            PlayFlashParticle(c);
        }

        foreach (ParticleSystem p in primaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
        }

        foreach (ParticleSystem p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
        }
    }

    void PlayFlashParticle(Color c)
    {
        GameObject.Find("CM vcam1").GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        foreach (ParticleSystem p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
            p.Play();
        }
    }

    private void Speed(float x)
    {
        currentSpeed = x;
    }

    void ChromaticAmount(float x)
    {
        postProfile.GetSetting<ChromaticAberration>().intensity.value = x;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    //}

    // PARA MÓVILES
    public void Accelerate()
    {
        direction = 1;
    }

    public void GoBackwards()
    {
        direction = -1;
    }

    public void NotMoveKart()
    {
        direction = 0;
    }

    public async void Jump()
    {
        //if (isGrounded)
        //{
        jumping = true;

        kartModel.parent.DOComplete();
        kartModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);
        await Task.Delay(1);
        //}
    }

    public void StopJumping()
    {
        print("DEJO DE SALTAR");
        jumping = false;
    }
}
