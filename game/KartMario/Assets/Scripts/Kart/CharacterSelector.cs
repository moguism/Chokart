using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] possibleCharacters; // Esta lista debe estar en el mismo orden que la est� en el selector de personajes

    public Texture[] textures; // Esta lista, exclusiva para la HUD y el minimap, tambi�n lo tiene que estar

    [SerializeField]
    private RawImage spriteImage;

    public KartController parent;

    public bool isHud = true;
    private readonly System.Random _random = new();

    private void Start()
    {
        // Para que no lo haga nada m�s empiece el selector
        if (CarSelection.hasFinished)
        {
            SetCharacter(CarSelection.characterIndex, true);
        }
    }

    public void SetCharacter(int index, bool destroy)
    {
        try
        {
            if(!isHud)
            {
                // Escoge personaje aleatorio
                if(parent != null && parent.enableAI)
                {
                    index = _random.Next(0, possibleCharacters.Length);
                }

                for (int i = 0; i < possibleCharacters.Length; i++)
                {
                    if (i == index) { continue; }

                    if (!destroy)
                    {
                        possibleCharacters[i].SetActive(false);
                    }
                    /*else
                    {
                        Destroy(possibleCharacters[i]);
                    }*/
                }

                GameObject active = possibleCharacters[index];
                active.SetActive(true);

                active.AddComponent<ClientNetworkTransform>();
                active.GetComponent<ClientNetworkTransform>().AuthorityMode = Unity.Netcode.Components.NetworkTransform.AuthorityModes.Owner;
            }

            SetSprite(index);
        }
        catch {}
    }

    private void SetSprite(int index)
    {
        if (spriteImage != null)
        {
            spriteImage.texture = textures[index];
        }
    }

    public Texture GetCharacterTexture(int index)
    {
        if (index >= 0 && index < textures.Length)
        {
            return textures[index];
        }
        return null; 
    }
}