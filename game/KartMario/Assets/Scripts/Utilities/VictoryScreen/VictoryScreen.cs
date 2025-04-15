using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private Canvas otherCanvas;

    [SerializeField]
    private GameObject container;

    /*[SerializeField]
    private float timer = 5.0f;*/

    [SerializeField]
    private GameObject playerItem;

    private float originalTime;

    public List<FinishKart> finishKarts = new();

    void Start()
    {
        originalTime = Time.timeScale;
        otherCanvas.enabled = false;
    }

    /*void Update()
    {
        if(timer >= 0.0f)
        {
            timer -= Time.deltaTime;

            float time = Time.timeScale == 1 ? .2f : 1;
            Time.timeScale = time;
        }
        else
        {
            Time.timeScale = originalTime;
        }
    }*/

    public void SetFinishKarts()
    {
        foreach(FinishKart kart in finishKarts)
        {
            GameObject item = Instantiate(playerItem, container.transform);

            VictoryScreenItem victoryScreenItem = item.GetComponentInChildren<VictoryScreenItem>();
            victoryScreenItem.finishKart = kart;
            victoryScreenItem.SetItem();
        }
    }

    public void CloseButton()
    {
        SceneManager.LoadScene(1); // Las lobbies
    }
}
