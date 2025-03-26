using System.Collections;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public float speed;

    void Start()
    {
        StartCoroutine(CalcSpeed());
    }

    // Calcula la velocidad de un objeto (los coche)
    IEnumerator CalcSpeed()
    {
        bool isPlaying = true;
        while (isPlaying)
        {
            Vector3 prevPos = transform.position;
            yield return new WaitForFixedUpdate();
            speed = Mathf.RoundToInt(Vector3.Distance(transform.position, prevPos) / Time.fixedDeltaTime);
        }
    }
}
