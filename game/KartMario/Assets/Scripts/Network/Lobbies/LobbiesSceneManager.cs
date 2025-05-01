using EasyTransition;
using Injecta;
using TMPro;
using UnityEngine;

public class LobbiesSceneManager : MonoBehaviour
{
    [Inject]
    private LobbyManager lobbyManager;

    [Inject]
    private WebsocketSingleton websocket;

    [SerializeField]
    private TransitionSettings transitionSettings;

    [SerializeField]
    private TMP_InputField joinCode;

    public static bool showError = false;

    [SerializeField]
    private TMP_Text placeholderText;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private GameObject playButtons;

    private Color initialColor;
    private string initialText;

    private async void Start()
    {
        initialColor = placeholderText.color;
        initialText = placeholderText.text;

        if(showError)
        {
            ChangePlaceholderValues(Color.red, "Fuiste expulsado");
            showError = false;
        }

        if(!WebsocketSingleton.connected)
        {
            await websocket.ConnectToSocket(AuthManager.token);
        }
    }

    public void CreateLobby()
    {
        //LobbyManager.PlayerName = "Testing";

        lobbyManager.CreateLobby();
        TransitionAndChangeScene();
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

        TransitionAndChangeScene();
    }

    private void TransitionAndChangeScene()
    {
        // Transiciono y voy a la selección de personajes

        CarSelection.audioSourceTime = audioSource.time;
        TransitionManager.Instance().Transition(4, transitionSettings, 0);
        //SceneManager.LoadScene(4); 
    }

    public void SetGameMode(int gamemode)
    {
        LobbyManager.gamemode = (Gamemodes)gamemode;
        playButtons.SetActive(true);
    }

    public void StartPlaying(bool spawnBots)
    {
        LobbyManager.spawnBotsWhenStarting = spawnBots;
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
