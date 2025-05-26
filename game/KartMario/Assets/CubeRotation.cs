using UnityEngine;

public class CubeRotation : MonoBehaviour
{
    public float floatStrength = 0.5f;   // cuando sube y baja 
    public float floatSpeed = 2f;        // velocidad a la q sube y baja
    public float rotationSpeed = 50f;    
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
