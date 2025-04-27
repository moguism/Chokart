using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] possibleCharacters; // Esta lista debe estar en el mismo orden que la está en el selector de personajes

    [SerializeField]
    private Texture[] textures; // Esta lista, exclusiva para la HUD, también lo tiene que estar

    [SerializeField]
    private RawImage spriteImage;

    private void Start()
    {
        // Para que no lo haga nada más empiece el selector
        if (WebsocketSingleton.kartModelIndex != -1)
        {

            SetCharacter(CarSelection.characterIndex, true);
        }
    }

    public void SetCharacter(int index, bool destroy)
    {
        try
        {
            if (spriteImage != null)
            {
                spriteImage.texture = textures[index];
            }
            else
            {
                for (int i = 0; i < possibleCharacters.Length; i++)
                {
                    if (i == index) { continue; }

                    if (!destroy)
                    {
                        possibleCharacters[i].SetActive(false);
                    }
                    else
                    {
                        Destroy(possibleCharacters[i]);
                    }
                }

                GameObject active = possibleCharacters[index];
                active.SetActive(true);

                active.AddComponent<ClientNetworkTransform>();
                active.GetComponent<ClientNetworkTransform>().AuthorityMode = Unity.Netcode.Components.NetworkTransform.AuthorityModes.Owner;
            }
        }
        catch {}
    }
}