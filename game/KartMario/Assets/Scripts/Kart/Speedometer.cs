using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Speedometer : MonoBehaviour
{
    public KartController kart;
    public RectTransform pointer;
    public TMP_Text speedText;

    public float maxKmH = 140f;
    public float maxSpeed = 140f;
    public float minRotation = -316.3f;
    public float maxRotation = -45.1f;
    public float smoothSpeed = 5f; // esto lo que hace es que la aguja gire progresivamente y no de golpe

    public float speed = 0f;

    private AudioSource audioSource;

    private Vector3 oldPosition;
    private const float minSpeed = 0.05f;

    private void Awake()
    {
        pointer = GameObject.Find("Pointer").GetComponent<RectTransform>();
        speedText = GameObject.Find("SpeedText").GetComponent<TMP_Text>();
        speedText.text = "0 km/h";
        audioSource = GameObject.Find("Driving").GetComponent<AudioSource>();
    }

    private void Update()
    {
        //print("velosidad : " + speed);
        // cuando va marcha atras

        /*if (speed < 0)
        {
            speed = -speed;
        }*/

        bool isMoving = false;

        if (kart == null || !kart.canMove)
        {
            speed = 0;
            audioSource.Stop();
            return;
        }

        //print("Velocidad actual: " + kart.currentSpeed);

        //la aguja gira suavemente
        speed = Mathf.Lerp(speed, kart.currentSpeed, Time.deltaTime * smoothSpeed);

        // velocidad entre 0 y 140 como el velocimetro
        float absSpeed = Mathf.Abs(speed);
        float visualSpeed = Mathf.Clamp(absSpeed * maxSpeed / maxKmH, -maxSpeed, maxSpeed);
        float rotationZ = Mathf.Lerp(maxRotation, minRotation, visualSpeed / maxSpeed);
        pointer.rotation = Quaternion.Euler(0, 0, rotationZ);

        // texto de velocidad cada 1 segundo para q no pete mucho
        if (speedText != null)
        {
            float distance = Vector3.Distance(oldPosition, kart.currentPosition);
            //print("Distancia: " + distance);

            if(Mathf.Abs(distance) >= 0.01)
            {
                isMoving = true;
            }

            if (distance > 0.01 && absSpeed < minSpeed)
            {
                speedText.text = "0,1 km/h";
            }
            else
            {
                if (absSpeed < 1f)
                {
                    speedText.text = absSpeed.ToString("F1") + " km/h";
                }
                else
                {
                    int roundedSpeed = Mathf.RoundToInt(absSpeed);
                    speedText.text = roundedSpeed.ToString() + " km/h";

                    try
                    {
#if UNITY_ANDROID
                        Handheld.Vibrate();
#else
                        Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);
#endif
                    }
                    catch {}
                }
            }
        }

        // Control de volumen según velocidad

        float targetVolume = Mathf.Clamp01(absSpeed / maxSpeed);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * smoothSpeed);

        if (isMoving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        oldPosition = kart.currentPosition;
    }
}
