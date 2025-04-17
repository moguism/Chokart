using Injecta;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbiesSceneManager : MonoBehaviour
{
    [Inject]
    private LobbyManager lobbyManager;

    [SerializeField]
    private TMP_InputField joinCode;

    [SerializeField]
    private Toggle spawnBotsToggle;

    public static bool showError = false;

    [SerializeField]
    private TMP_Text errorText;

    private void Start()
    {
        if(showError)
        {
            errorText.enabled = true;
            showError = false;
        }
    }

    public void CreateLobby()
    {
        //LobbyManager.PlayerName = "Testing";

        lobbyManager.CreateLobby();
        LobbyManager.spawnBotsWhenStarting = spawnBotsToggle.isOn;
        SceneManager.LoadScene(3); // La selección de personajes
    }

    public void JoinLobby()
    {
        //LobbyManager.PlayerName = "Testing";

        lobbyManager.JoinLobbyByCode(joinCode.text);
        SceneManager.LoadScene(3);
    }
}
