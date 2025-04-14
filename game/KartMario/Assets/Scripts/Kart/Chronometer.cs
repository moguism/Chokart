using System;
using System.Collections;
using TMPro;
using UnityEngine;

// el cronometro
public class Chronometer : MonoBehaviour
{
    public static Chronometer instance;
    public TMP_Text timerText;
    private string endTime;
    private TimeSpan timeChronometer;
    public bool timerOn;
    private float timePass;

    private void Awake()
    {
        instance = this;
        timerText = GameObject.Find("Chronometer").GetComponent<TMP_Text>();
        timerText.text = "00:00:00";
    }

    void Start()
    {
        timerText.text = "00:00:00";
        timerOn = false;
    }

    public void StartTimer()
    {
        timerOn = true;
        timePass = 0f;

        StartCoroutine(ActUpdate());
    }

    public void StopTimer()
    {
        endTime = timerText.text;
        timerOn = false;
    }

    private IEnumerator ActUpdate()
    {
        while (timerOn)
        {
            timePass += Time.deltaTime;
            timeChronometer = TimeSpan.FromSeconds(timePass);
            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", timeChronometer.Minutes, timeChronometer.Seconds, timeChronometer.Milliseconds);

            //Debug.Log("segundos cronometro: "+ timeChronometer.Seconds);


            yield return null;
        }
    }
}
