using EasyTransition;
using System.Collections.Generic;
using Unity.Netcode;
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

    /*void Start()
    {
        gameObject.SetActive(false);
        return;
    }*/

    public void SetFinishKarts()
    {
        otherCanvas.enabled = false;

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
        otherCanvas.enabled = true;
        NetworkManager.Singleton.Shutdown();
        TransitionManager.Instance().Transition(2, transitionSettings, 0); // Las lobbies
    }
}
