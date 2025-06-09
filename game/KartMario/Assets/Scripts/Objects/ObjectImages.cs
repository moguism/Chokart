using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectImages : MonoBehaviour
{
    [Header("Referencias")]
    public Image objectIconImage;
    public Image objectEffectImage;

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

    [Header("Efectos de objetos")]
    public Sprite invisivilityEffect;
    public Sprite distorsionEffect;
    public Sprite invulnerabilityEffect;


    private Dictionary<string, Sprite> objectSprites;
    private Dictionary<string, Sprite> objectEffects;

    private float timer = 10f;

    private bool enableEffect = false;

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

        objectEffects = new Dictionary<string, Sprite>
        {
            { "Invisibilidad", invisivilityEffect },
            { "Ataque distorsivo", distorsionEffect },
            { "Protección", invulnerabilityEffect },
        };

        UpdateObjectIcon("None");
        UpdateObjectEffect("None");

    }

    private void Update()
    {
        if (enableEffect)
        {
            timer -= Time.deltaTime;

            if(timer <0){
                enableEffect = false;
                timer = 10f;
                UpdateObjectEffect("None");
            }
        }
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

    public void UpdateObjectEffect(string objectName)
    {
        Debug.Log("efecto OBJETO: " + objectName);

        if (objectEffects.ContainsKey(objectName))
        {
            objectEffectImage.sprite = objectEffects[objectName];
            objectEffectImage.enabled = true;
            enableEffect = true;
        }
        else
        {
            objectEffectImage.enabled = false;
        }

    }
}
