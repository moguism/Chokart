using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

// dibuja el minimapa a partir de los triggers
public class NewMonoBehaviourScript : MonoBehaviour
{

    public Transform triggerParent; // El objeto que contiene los triggers
    public UILineRenderer lineRenderer; // dibuja el minimapa
    public Transform origin; // Centro de la pista
    public float mapScale = 1f;

    void Start()
    {
        List<Vector2> points = new List<Vector2>();

        foreach (Transform child in triggerParent)
        {
            Vector3 offset = child.position - origin.position;
            Vector2 point = new Vector2(offset.x, offset.z) * mapScale;
            points.Add(point);
        }

        if (points.Count > 1)
        {
            points.Add(points[0]); // para q cierre el dibujo añade el primer trigger al final
        }

        lineRenderer.Points = points.ToArray();
        lineRenderer.SetAllDirty(); 

    }
    }

