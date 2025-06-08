using TMPro;
using UnityEngine;

public class ShowCodeText : MonoBehaviour
{
    [SerializeField]
    public TMP_Text codeText;
    private string copiedText = "";
    private string joinCodeText = "";
    private bool shouldChangeCode = false;
    private float timerCode = 2.0f;
    private float maxTimer;

    private void Awake()
    {
        if (codeText == null) codeText = GameObject.Find("JoinCode").GetComponent<TMP_Text>();
    }
    void Start()
    {

        switch (LocalizationManager.languageCode)
        {
            case "es-ES":
                copiedText = "Copiado :D";
                break;
            case "en-US":
                copiedText = "Copied :D";
                break;
        }

        codeText.text =  LobbyManager.lobbyCode;
    }

    void Update()
    {
        if (LobbyManager.gameStarted || LobbyManager.lobbyCode == "" | LobbyManager.lobbyCode == null)
        {
            gameObject.SetActive(false); 
            return;
        }
        if (shouldChangeCode)
        {
            timerCode -= Time.deltaTime;
            if (timerCode <= 0.0f)
            {
                codeText.text = joinCodeText + LobbyManager.lobbyCode;
                timerCode = maxTimer;
                shouldChangeCode = false;
            }
        }

    }

    public void CopyCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = LobbyManager.lobbyCode;
        codeText.text = copiedText;
        shouldChangeCode = true;
    }

}
