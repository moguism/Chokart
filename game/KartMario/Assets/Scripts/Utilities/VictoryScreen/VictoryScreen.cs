using EasyTransition;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField]
    private PauseScreen pauseScreen;

    public List<FinishKart> finishKarts = new();

    [Header("Timer")]
    private const float MAX_TIMER = 10;
    private float timer;

    [SerializeField]
    private TMP_Text timerText;

    void Start()
    {
        timer = MAX_TIMER;
    }

    private void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.RoundToInt(timer).ToString();

            if(timer <= 0.0f)
            {
                CloseButton();
            }
        }
        else
        {
            timer = MAX_TIMER;
        }
    }

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

        pauseScreen.DisableControls();

        TransitionManager.Instance().Transition(2, transitionSettings, 0); // Las lobbies
    }
}
