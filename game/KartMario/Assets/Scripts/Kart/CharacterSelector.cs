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
            SetCharacter(CarSelection.characterIndex);
        }
    }

    public void SetCharacter(int index)
    {
        try
        {
            for (int i = 0; i < possibleCharacters.Length; i++)
            {
                possibleCharacters[i].SetActive(false);
            }
            possibleCharacters[index].SetActive(true);
        }
        catch {}
    }
}