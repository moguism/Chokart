using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectImages : MonoBehaviour
{
    [Header("Referencias")]
    public Image objectIconImage;

    [Header("Imagenes de objetos")]
    public Sprite defaultSprite;
    public Sprite greenShellSprite;
    public Sprite bombSprite;
    public Sprite invisivilitySprite;
    public Sprite positionChangerSprite;
    public Sprite distorsionSprite;
    public Sprite healthPotionSprite;
    public Sprite invulnerabilitySprite;
    public Sprite speedBoostSprite;


    private Dictionary<string, Sprite> objectSprites;

    void Start()
    {
        objectSprites = new Dictionary<string, Sprite>
        {
            { "Proyectil", greenShellSprite },
            { "Bomba", bombSprite },
            { "Invisibilidad", invisivilitySprite },
            { "Cambiar posición", positionChangerSprite },
            { "Ataque distorsivo", distorsionSprite },
            { "Poción de vida", healthPotionSprite },
            { "Protección", invulnerabilitySprite },
            { "Gorrocoptero", speedBoostSprite }
        };

        UpdateObjectIcon("None");
    }

    public void UpdateObjectIcon(string objectName)
    {
        Debug.Log("IMAGEN OBJETO: " + objectName);
        if (objectSprites.ContainsKey(objectName))
        {
            objectIconImage.sprite = objectSprites[objectName];
            objectIconImage.enabled = true;
        }
        else
        {
            objectIconImage.enabled = false;
        }
    }
}
