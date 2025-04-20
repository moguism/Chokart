using System;
using TMPro;
using UnityEngine;

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

        if (speed < 0)
        {
            speed = -speed;
        }

        if (kart == null || !kart.canMove)
        {
            speed = 0;
            kart.currentSpeed = 0;
            audioSource.Stop();
            return;
        }

        //la aguja gira suavemente
        speed = Mathf.Lerp(speed, kart.currentSpeed, Time.deltaTime * smoothSpeed);

        // velocidad entre 0 y 140 como el velocimetro
        float visualSpeed = Mathf.Clamp(speed * maxSpeed / maxKmH, 0, maxSpeed);
        float rotationZ = Mathf.Lerp(maxRotation, minRotation, visualSpeed / maxSpeed);

        pointer.rotation = Quaternion.Euler(0, 0, rotationZ);

        // texto de velocidad cada 1 segundo para q no pete mucho
        if (speedText != null)
        {
            int roundedSpeed = Mathf.RoundToInt(speed);
            speedText.text = roundedSpeed.ToString() + " km/h";
        }

        // Control de volumen según velocidad

        float targetVolume = Mathf.Clamp01(speed / maxSpeed);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * smoothSpeed);

        if (speed > 0 && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        if (speed < 0.5f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
