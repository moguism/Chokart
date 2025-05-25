using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    [Header("Configuración")]
    public RectTransform minimapCanvas;
    public Transform origin;
    public float mapScale = 1f;

    [Header("Icono")]
    public GameObject minimapDotPrefab; // Prefab punto rojo

    private readonly List<RectTransform> minimapDots = new List<RectTransform>();
    private readonly List<Transform> playerCars = new List<Transform>();

    private List<KartController> clients = new List<KartController>();

    private CharacterSelector characterSelector;

    public void UpdatePlayerPositions()
    {
        playerCars.Clear();

        for(int i = 0; i < minimapDots.Count; i++)
        {
            Destroy(minimapDots[i].gameObject);
        }

        minimapDots.Clear();

        clients = FindObjectsByType<KartController>(FindObjectsSortMode.None).ToList();

        foreach (var client in clients)
        {

            playerCars.Add(client.transform);
            CreateMinimapDotForPlayer(client.transform, client.characterIndex);
        }
    }

    private void CreateMinimapDotForPlayer(Transform playerCar, int characterIndex)
    {
        if (!minimapDots.Exists(dot => dot.transform.Equals(playerCar)))
        {
            // agrega punto rojo al canvas
            GameObject dot = Instantiate(minimapDotPrefab, minimapCanvas);

            if(characterSelector == null)
            {
                characterSelector = FindObjectsByType<CharacterSelector>(FindObjectsSortMode.None).FirstOrDefault(ch => ch.isHud);
            }

            dot.GetComponent<RawImage>().texture = characterSelector.textures[characterIndex];

            RectTransform dotRectTransform = dot.GetComponent<RectTransform>();
            minimapDots.Add(dotRectTransform);

            Debug.Log("Minimap Dot instanciado para: " + playerCar.name);
        }
    }

    private void Update()
    {
        try
        {
            if (playerCars.Count == 0 || minimapDots.Count == 0) return;

            for (int i = 0; i < playerCars.Count; i++)
            {
                Transform playerCar = playerCars[i];
                RectTransform dot = minimapDots[i];

                // posición relativa del jugador con respecto al origen
                Vector3 offset = playerCar.position - origin.position;
                Vector2 minimapPos = new Vector2(offset.x, offset.z) * mapScale;

                dot.anchoredPosition = minimapPos;
            }
        }
        catch { }
    }
}
