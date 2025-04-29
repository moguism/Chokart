using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] GameObject components;
    [SerializeField] GameObject minimap;

    private InputSystem_Actions inputActions;
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
        minimap.SetActive(!show);
        components.SetActive(show);
        isChatActive = show;
    }
}
