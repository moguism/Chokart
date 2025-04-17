using Injecta;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbiesSceneManager : MonoBehaviour
{
    [Inject]
    private LobbyManager lobbyManager;

    [SerializeField]
    private TMP_InputField joinCode;

    public static bool showError = false;

    [SerializeField]
    private TMP_Text placeholderText;

    private Color initialColor;
    private string initialText;

    private void Start()
    {
        initialColor = placeholderText.color;
        initialText = placeholderText.text;

        if(showError)
        {
            ChangePlaceholderValues(Color.red, "Fuiste expulsado");
            showError = false;
        }
    }

    public void CreateLobby()
    {
        //LobbyManager.PlayerName = "Testing";

        lobbyManager.CreateLobby();
        SceneManager.LoadScene(3); // La selección de personajes
    }

    public async void JoinLobby()
    {
        //LobbyManager.PlayerName = "Testing";

        bool joined = await lobbyManager.JoinLobbyByCode(joinCode.text);
        if(!joined)
        {
            ChangePlaceholderValues(Color.red, "No hay lobbies");
            return;
        }
        SceneManager.LoadScene(3);
    }

    public void PlayWithBots()
    {
        LobbyManager.spawnBotsWhenStarting = true;
        CreateLobby();
    }

    public void PlayWithoutBots()
    {
        LobbyManager.spawnBotsWhenStarting = false;
        CreateLobby();
    }

    public void OnLobbyCodeValueChanged()
    {
        ChangePlaceholderValues(initialColor, initialText);
    }

    private void ChangePlaceholderValues(Color color, string text)
    {
        placeholderText.color = color;
        placeholderText.text = text;
    }
}
