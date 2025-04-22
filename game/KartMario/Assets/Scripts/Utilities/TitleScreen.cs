using EasyTransition;
using Injecta;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleScreen : MonoBehaviour
{
    [Header("Kart")]
    [SerializeField]
    private float moveFactor;

    [SerializeField]
    private float kartLimit;

    [SerializeField]
    private GameObject kart;

    [Header("Base Objects")]
    [SerializeField]
    private GameObject logo;

    [SerializeField]
    private GameObject brokenScreen;

    [Header("Images")]
    [SerializeField]
    private RawImage imageLogo;

    [SerializeField]
    private GameObject backgroundImage;

    [SerializeField]
    private VideoPlayer backgroundVideo;

    [Header("Frames")]
    [SerializeField]
    private Texture[] logoFrames;

    private int indexLogo = 0;
    private bool doLogoCountdown = false;

    [Header("Timers")]
    [SerializeField]
    private float maxTimerLogo = 0.0761f;
    private float gifTimerLogo;

    private bool hasFinished = false;
    private bool hasWaited = false;

    [Header("Audios")]
    [SerializeField]
    private AudioClip music;

    [SerializeField]
    private AudioClip crash;

    [SerializeField]
    private AudioClip driving;

    [SerializeField]
    private AudioSource audioSource;

    [Inject]
    private AuthManager authManager;

    private int playKartIndex;

    [Header("Buttons")]
    [SerializeField]
    private GameObject logoutButton;

    [SerializeField]
    private GameObject buttons;

    [SerializeField]
    private TransitionSettings transitionSettings;

    private int indexToPlayMusic;

    void Start()
    {
        gifTimerLogo = maxTimerLogo;
        playKartIndex = Mathf.RoundToInt(kartLimit + 10);

        indexToPlayMusic = Mathf.RoundToInt(logoFrames.Length / 2);
    }

    async void Update()
    {
        if (!authManager.isTryingToLog)
        {
            if (!authManager.isLogged)
            {
                Destroy(logoutButton);
            }
        }

        if (hasFinished)
        {
            if (brokenScreen.activeInHierarchy)
            {
                brokenScreen.SetActive(false);
                backgroundImage.SetActive(false);

                buttons.SetActive(true);

                backgroundVideo.Play();
            }
            return;
        }

        kart.transform.position = new Vector3(kart.transform.position.x, kart.transform.position.y, kart.transform.position.z + moveFactor);

        if (!doLogoCountdown)
        {
            if (Mathf.RoundToInt(kart.transform.position.z) == playKartIndex)
            {
                PlayAudio(crash, false);
            }
            else if (kart.transform.position.z <= kartLimit)
            {
                brokenScreen.SetActive(true);
                doLogoCountdown = true;
            }
        }
        else
        {
            if (!hasWaited)
            {
                await Task.Delay(1000);
                hasWaited = true;
            }

            if(indexLogo == indexToPlayMusic)
            {
                PlayAudio(music, true);
            }

            logo.SetActive(true);
            doLogoCountdown = true;
            DoLogoCountdown();
        }
    }

    private void PlayAudio(AudioClip clip, bool loop)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    private void DoLogoCountdown()
    {
        gifTimerLogo -= Time.deltaTime;

        if (gifTimerLogo <= 0.0f)
        {
            if (indexLogo < logoFrames.Length)
            {
                imageLogo.texture = logoFrames[indexLogo];
                indexLogo++;
            }
            else
            {
                hasFinished = true;
                return;
            }

            gifTimerLogo = maxTimerLogo;
        }
    }

    public void StartGame()
    {
        if (!authManager.isTryingToLog)
        {
            if (!authManager.isLogged)
            {
                AuthManagerScene.audioSourceTime = audioSource.time;
                AuthManagerScene.videoTime = backgroundVideo.time;
                TransitionManager.Instance().Transition(1, transitionSettings, 0);
            }
            else
            {
                TransitionManager.Instance().Transition(2, transitionSettings, 0);
            }
        }
    }

    public void LogoutButton()
    {
        authManager.Logout();
        Destroy(logoutButton);
    }
}
