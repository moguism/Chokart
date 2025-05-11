using Cysharp.Threading.Tasks;
using Injecta;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelection : MonoBehaviour
{
    public static CharacterModel[] characters;

    [SerializeField]
    private CharacterModel[] _characters;

    [SerializeField]
    private TMP_Text speedText;

    [SerializeField]
    private GameObject kartBase;

    [SerializeField]
    private CustomVideoPlayer videoPlayer;

    [SerializeField]
    private RawImage backgroundImage;

    [SerializeField]
    private RawImage videoRawImage;
    private Color startVideoColor;

    private int index;
    public static int characterIndex = 0; // En este caso, como el personaje es un cosmético no hace falta guardarlo en el websocket (simplemente se instancia encima del coche)

    private bool showingCharacters = false;

    [SerializeField]
    private CustomVideoPlayer glitchPlayer;

    [SerializeField]
    private AudioSource glitchAudio;

    [Inject]
    private LobbyManager lobbyManager;

    [SerializeField]
    private TMP_Text joinCodeText;
    private bool shouldChangeCode = false;

    private string originalText = "";
    private string waitingText = "";
    private string speedString = "";

    private string otherText;
    private float timerCode = 2.0f;
    private float maxTimer;

    [SerializeField]
    private TMP_Text buttonText;

    [SerializeField]
    private AudioSource audioSource;

    public static bool hasFinished = false;
    private bool waiting = false;

    public static float audioSourceTime;

    [SerializeField]
    private GameObject colorSelection;

    [SerializeField]
    private Material kartMaterial;

    [SerializeField]
    private GameObject previousButton;

    [SerializeField]
    private GameObject nextButton;

    [SerializeField]
    private GameObject kart;

    private void Start()
    {
        print(lobbyManager);
        maxTimer = timerCode;

        audioSource.time = audioSourceTime + 0.7f;
        audioSource.Play();

        if (LobbyManager.lobbyCode == "" || LobbyManager.lobbyCode == null)
        {
            Destroy(joinCodeText.transform.parent.gameObject);
        }
        else
        {
            joinCodeText.text = LobbyManager.lobbyCode;
        }

        characters = _characters.ToArray(); // Para que haga una copia

        videoPlayer.videoPlayer.SetDirectAudioVolume(0, 0.25f); // Para el volumen
        startVideoColor = videoRawImage.color;

        index = PlayerPrefs.GetInt("carIndex");
        characterIndex = PlayerPrefs.GetInt("characterIndex");

        switch (LocalizationManager.languageCode)
        {
            case "es-ES":
                otherText = "Copiado :D";
                waitingText = "ESPERANDO...";
                speedString = "VELOCIDAD: ";
                break;
            case "en-US":
                otherText = "Copied :D";
                waitingText = "WAITING...";
                speedString = "SPEED: ";
                break;
        }
    }

    private void FixedUpdate()
    {
        kartBase.transform.Rotate(0, 1f, 0);
    }

    private void Update()
    {
        if (shouldChangeCode)
        {
            timerCode -= Time.deltaTime;
            if (timerCode <= 0.0f)
            {
                joinCodeText.text = originalText;
                timerCode = maxTimer;
                shouldChangeCode = false;
            }
        }

        if (hasFinished && lobbyManager.hasRelay)
        {
            SceneManager.LoadScene(3);
        }
    }

    public void Next()
    {
        if (waiting)
        {
            return;
        }

        characterIndex++;
        if (characterIndex >= _characters.Length)
        {
            characterIndex = 0;
        }

        ManageVisibilityAndSave();
    }

    public void Prev()
    {
        if (waiting)
        {
            return;
        }


        characterIndex--;
        if (characterIndex < 0)
        {
            characterIndex = _characters.Length - 1;
        }

        ManageVisibilityAndSave();
    }

    public async void GoToGame()
    {
        if (waiting)
        {
            return;
        }

        if (showingCharacters)
        {
            hasFinished = true;

            bool joined = await lobbyManager.StartRelay();
            if (joined)
            {
                videoPlayer.videoPlayer.url = null;
                videoPlayer.videoPlayer.targetTexture.Release();
                SceneManager.LoadScene(3); // El juego
            }
            else
            {
                buttonText.text = waitingText;
                waiting = true;
            }
        }
        else
        {
            colorSelection.SetActive(false);
            previousButton.SetActive(true);
            nextButton.SetActive(true);
            audioSource.Stop();
            showingCharacters = true;
            ManageVisibilityAndSave();
        }
    }

    private void ManageVisibilityAndSave()
    {
        if (showingCharacters)
        {
            ManageCharacterVisibility();
        }

        SaveIndex();
    }

    private async void ManageCharacterVisibility()
    {
        try
        {
            kart.GetComponentInChildren<CharacterSelector>().SetCharacter(characterIndex, false);

            speedText.text = _characters[characterIndex].name;

            if (videoPlayer.videoPlayer.url == null || videoPlayer.videoPlayer.url == "")
            {
                glitchAudio.Play();
                glitchPlayer.PlayVideo();

                await UniTask.WaitForSeconds(1);

                glitchPlayer.videoPlayer.enabled = false;
                glitchAudio.Stop();
            }

            string clip = _characters[characterIndex].clip;
            if (clip == null || clip == "")
            {
                videoPlayer.videoPlayer.Stop();
                Debug.LogWarning("No hay video :(");
                SetVideoAndImageAvailability(true, false);
                return;
            }

            SetVideoAndImageAvailability(false, true);
            videoPlayer.videoFileName = clip;
            videoPlayer.PlayVideo();
        }
        catch { }
    }

    public void OnColorSelected(Color color)
    {
        kartMaterial.color = color;
    }

    private void SetVideoAndImageAvailability(bool imageOptions, bool videoOptions)
    {
        //backgroundImage.color = imageOptions ? Color.white : Color.black;
        videoRawImage.color = videoOptions ? startVideoColor : new Color(videoRawImage.color.r, videoRawImage.color.g, videoRawImage.color.b, 0);
    }

    private void SaveIndex()
    {
        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.SetInt("characterIndex", characterIndex);
        PlayerPrefs.Save();
    }

    public void CopyCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = joinCodeText.text;
        originalText = joinCodeText.text;
        shouldChangeCode = true;
        joinCodeText.text = otherText;
    }
}

[System.Serializable]
public class CharacterModel
{
    public string name;
    public string clip;
}