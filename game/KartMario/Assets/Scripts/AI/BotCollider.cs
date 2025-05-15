using UnityEngine;
using System.Collections.Generic;

public class BotCollider : MonoBehaviour
{
    [SerializeField] private KartController parent;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject hair;
    [SerializeField] private List<GameObject> self;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float maxRotationAngle = 45f;

    private Quaternion originalHeadRotation;
    private Quaternion originalHairRotation;

    private Quaternion targetHeadRotation;
    private Quaternion targetHairRotation;
    private bool isRotating = false;
    private bool shouldRotate = false;

    private float rotationTimer = 1f;
    private float maxRotationTimer;

    private void Start()
    {
        originalHeadRotation = head.transform.rotation;
        originalHairRotation = hair.transform.rotation;
        targetHeadRotation = originalHeadRotation;
        targetHairRotation = originalHairRotation;
        maxRotationTimer = rotationTimer;
    }

    private void Update()
    {
        if (isRotating)
        {
            head.transform.rotation = Quaternion.Slerp(head.transform.rotation, targetHeadRotation, Time.deltaTime * rotationSpeed);
            hair.transform.rotation = Quaternion.Slerp(hair.transform.rotation, targetHairRotation, Time.deltaTime * rotationSpeed);
            shouldRotate = true;
        }
        else if(parent.horizontalInput == 0 && shouldRotate)
        {
            rotationTimer -= Time.deltaTime;

            head.transform.rotation = Quaternion.Slerp(hair.transform.rotation, originalHeadRotation, Time.deltaTime * rotationSpeed);
            hair.transform.rotation = Quaternion.Slerp(hair.transform.rotation, originalHairRotation, Time.deltaTime * rotationSpeed);

            if(rotationTimer <= 0.0f)
            {
                rotationTimer = maxRotationTimer;
                shouldRotate = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart") && !self.Contains(other.gameObject))
        {
            //Debug.LogError(other);
            Vector3 direction = other.transform.position - transform.position;
            if (direction != Vector3.zero)
            {
                //Quaternion lookRotation = Quaternion.FromToRotation(transform.forward, direction);
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                // Limit the rotation based on the original rotation
                targetHeadRotation = LimitRotation(originalHeadRotation, lookRotation, maxRotationAngle);
                targetHairRotation = LimitRotation(originalHairRotation, lookRotation, maxRotationAngle);

                isRotating = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isRotating = false;
    }

    private Quaternion LimitRotation(Quaternion original, Quaternion target, float maxAngle)
    {
        float angle = Quaternion.Angle(original, target);
        if (angle > maxAngle)
        {
            float t = maxAngle / angle;
            return Quaternion.Slerp(original, target, t);
        }
        return target;
    }
}
