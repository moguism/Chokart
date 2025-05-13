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

    [SerializeField]
    private ChatManager chatManager;

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
        DisableControls();
        NetworkManager.Singleton.Shutdown();
        TransitionManager.Instance().Transition(2, transition, 0);
    }

    private void DisableControls()
    {
        try
        {
            kart.playerControls.Player.Disable();
            kart.playerControls.Mobile.Disable();
            kart.playerControls.UI.Disable();
            kart.playerControls.Disable();

            chatManager.inputActions.Player.Disable();
            chatManager.inputActions.Mobile.Disable();
            chatManager.inputActions.UI.Disable();
            chatManager.inputActions.Disable();
        }
        catch { }
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
