using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] possibleCharacters; // Esta lista debe estar en el mismo orden que la está en el selector de personajes

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
            possibleCharacters[index].SetActive(true);
        }
        catch {}
    }
}