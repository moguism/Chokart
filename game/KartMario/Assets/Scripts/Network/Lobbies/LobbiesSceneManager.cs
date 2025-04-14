using Injecta;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbiesSceneManager : MonoBehaviour
{
    [Inject]
    private LobbyManager lobbyManager;

    [SerializeField]
    private TMP_InputField joinCode;

    [SerializeField]
    private TMP_InputField playerName;

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
        lobbyManager.CreateLobby(playerName.text);
        SceneManager.LoadScene(3); // La selección de personajes
    }

    public void JoinLobby()
    {
        lobbyManager.JoinLobbyByCode(joinCode.text, playerName.text);
        SceneManager.LoadScene(3);
    }
}
