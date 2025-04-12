using EZhex1991.EZSoftBone;
using UnityEngine;

public class HairPhysics : MonoBehaviour
{
    [SerializeField]
    private EZSoftBone hairPhysics;

    private readonly System.Random _random = new();

    [SerializeField]
    private KartController parent;

    private Vector3 oldPosition;

    void Update()
    {
        //print("Velocidad del padre: " + parent.currentSpeed);

        float distance = Vector3.Distance(oldPosition, parent.currentPosition);
        //print("Posición actual: " + parent.currentPosition + ". Posición antigua: " + oldPosition + ". Distancia: " + distance);

        if (distance > 0.09)
        {
            hairPhysics.gravity = new Vector3(hairPhysics.gravity.x, _random.Next(0, 5), _random.Next(0, 5));
        }
        else
        {
            hairPhysics.gravity = new Vector3(0, 0, 0);
        }

        oldPosition = parent.currentPosition;

    }
}
