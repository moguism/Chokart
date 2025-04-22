using EasyTransition;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private Canvas otherCanvas;

    [SerializeField]
    private GameObject container;

    [SerializeField]
    private TransitionSettings transitionSettings;

    [SerializeField]
    private GameObject playerItem;

    public List<FinishKart> finishKarts = new();

    public static bool disable = true;
    public static bool showing = false;

    void Start()
    {
        if (disable)
        {
            gameObject.SetActive(false);
            disable = false;
            return;
        }
        otherCanvas.enabled = false;
        disable = true;
    }

    public void SetFinishKarts()
    {
        LobbyManager.gameStarted = false;
        showing = true;

        foreach (FinishKart kart in finishKarts)
        {
            GameObject item = Instantiate(playerItem, container.transform);

            VictoryScreenItem victoryScreenItem = item.GetComponentInChildren<VictoryScreenItem>();
            victoryScreenItem.finishKart = kart;
            victoryScreenItem.SetItem();
        }
    }

    public void CloseButton()
    {
        print("Cerrando...");
        TransitionManager.Instance().Transition(2, transitionSettings, 0); // Las lobbies
    }
}
