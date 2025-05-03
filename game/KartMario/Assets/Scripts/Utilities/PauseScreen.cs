using EasyTransition;
using Unity.Netcode;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject screen;

    [SerializeField]
    private GameObject minimap;

    [SerializeField]
    private TransitionSettings transition;

    [SerializeField]
    private PositionManager positionManager;

    [SerializeField]
    private GameObject hud;

    [SerializeField]
    private GameObject pauseButton;

    public KartController kart;

    public void Open()
    {
        SetAvailability(false, true);
    }

    public void Continue()
    {
        SetAvailability(true, false);
    }

    public async void ExitGame()
    {
        await positionManager.ExitGame();
        NetworkManager.Singleton.Shutdown();
        TransitionManager.Instance().Transition(2, transition, 0);
    }

    private void SetAvailability(bool map, bool pause)
    {
        minimap.SetActive(map);
        pauseButton.SetActive(map);
        hud.SetActive(map);
        kart.canMove = map;

        screen.SetActive(pause);
    }
}
