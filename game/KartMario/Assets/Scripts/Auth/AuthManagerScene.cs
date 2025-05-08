using EasyTransition;
using Injecta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthManagerScene : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField emailOrNickname;

    [SerializeField]
    private TMP_InputField password;

    [SerializeField]
    private Toggle rememberMe;

    [SerializeField]
    private GameObject errorMessage;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private CustomVideoPlayer videoPlayer;

    [SerializeField]
    private TransitionSettings transitionSettings;

    [Inject]
    private AuthManager authManager;

    /*[Inject]
    private WebsocketSingleton websocket;*/

    private bool isLogin = false;

    public static float audioSourceTime;
    public static double videoTime;

    private void Start()
    {
        audioSource.time = audioSourceTime + 0.7f;
        videoPlayer.videoPlayer.time  = videoTime + 0.7f;

        audioSource.Play();
        videoPlayer.PlayVideo();
    }

    public async void Login()
    {
        if(isLogin)
        {
            return;
        }

        isLogin = true;

        bool couldLogin = await authManager.LoginAsync(emailOrNickname.text, password.text, rememberMe.isOn);
        if(!couldLogin)
        {
            errorMessage.SetActive(true);
        }
        else
        {
            TransitionManager.Instance().Transition(2, transitionSettings, 0);
        }

        isLogin = false;
    }
}
