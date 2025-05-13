using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    [Header("Panel")]
    [SerializeField]
    private Image panel;

    [SerializeField]
    private Color activePanelColor;

    [SerializeField]
    private Color inactivePanelColor;

    [Header("Input")]
    [SerializeField] 
    private TMP_InputField chatInput;

    [SerializeField]
    private GameObject chatInputObject;

    [Header("Scrollbar")]
    [SerializeField]
    private Image scrollbarFirstImage;

    [SerializeField]
    private Image scrollbarSecondImage;

    [Header("Otras opciones")]
    [SerializeField]
    private ChatMessage chatMessagePrefab;

    [SerializeField]
    private CanvasGroup chatContent;

    [SerializeField]
    private GameObject closeButton;

    public InputSystem_Actions inputActions;
    private string playerName;

    public static bool isChatActive = false;

    private void Start()
    {
        playerName = LobbyManager.PlayerName;

        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    void Update() 
    {
        if(isChatActive && inputActions.UI.Submit.ReadValue<float>() == 1)
        {
            SendMessage();
        }
    }

    public void SendMessage()
    {
        SendChatMessage(chatInput.text, playerName);
        chatInput.text = "";
    }

    private void SendChatMessage(string _message, string _fromWho = null)
    { 
        if(string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " > " +  _message;
        SendChatMessageServerRpc(S); 
    }
   
    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message)
    {
        AddMessage(message);
    }

    public void ShowChat(bool show)
    {
        isChatActive = show;

        scrollbarFirstImage.enabled = isChatActive;
        scrollbarSecondImage.enabled = isChatActive;

        chatInputObject.SetActive(isChatActive);
        closeButton.SetActive(isChatActive);

        panel.color = isChatActive ? activePanelColor : inactivePanelColor;
    }
}
