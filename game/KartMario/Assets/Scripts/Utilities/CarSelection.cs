using Cysharp.Threading.Tasks;
using Injecta;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CarSelection : MonoBehaviour
{
    public static KartModel[] cars;
    public static CharacterModel[] characters;

    [SerializeField]
    private KartModel[] _cars;

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
    public static int characterIndex; // En este caso, como el personaje es un cosmético no hace falta guardarlo en el websocket (simplemente se instancia encima del coche)

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

    private bool hasFinished = false;
    private bool waiting = false;

    public static float audioSourceTime;

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

        cars = _cars.ToArray(); // Para que haga una copia
        characters = _characters.ToArray();

        videoPlayer.videoPlayer.SetDirectAudioVolume(0, 0.25f); // Para el volumen
        startVideoColor = videoRawImage.color;

        index = PlayerPrefs.GetInt("carIndex");
        characterIndex = PlayerPrefs.GetInt("characterIndex");
        ManageCarVisibility();

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
        if(shouldChangeCode)
        {
            timerCode -= Time.deltaTime;
            if(timerCode <= 0.0f)
            {
                joinCodeText.text = originalText;
                timerCode = maxTimer;
                shouldChangeCode = false;
            }
        }

        if(hasFinished && lobbyManager.hasRelay)
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

        if (!showingCharacters)
        {
            index++;
            if (index >= _cars.Length)
            {
                index = 0;
            }
        }
        else
        {
            characterIndex++;
            if (characterIndex >= _characters.Length)
            {
                characterIndex = 0;
            }
        }
        ManageVisibilityAndSave();
    }

    public void Prev()
    {
        if(waiting)
        {
            return;
        }

        if (!showingCharacters)
        {
            index--;
            if (index < 0)
            {
                index = _cars.Length - 1;
            }
        }
        else
        {
            characterIndex--;
            if (characterIndex < 0)
            {
                characterIndex = _characters.Length - 1;
            }
        }
        ManageVisibilityAndSave();
    }

    public async void GoToGame()
    {
        if(waiting)
        {
            return;
        }

        WebsocketSingleton.kartModelIndex = index;

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
        else
        {
            ManageCarVisibility();
        }

        SaveIndex();
    }

    private void ManageCarVisibility()
    {
        for (int i = 0; i < _cars.Length; i++)
        {
            _cars[i].car.SetActive(false);
        }

        _cars[index].car.SetActive(true);
        //speedText.text = speedString + _cars[index].speed;
    }

    private async void ManageCharacterVisibility()
    {
        try
        {
            _cars[index].car.GetComponentInChildren<CharacterSelector>().SetCharacter(characterIndex, false);

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
public class KartModel
{
    public GameObject car; // Por ahora esto es una referencia al modelo (digo por ahora porque probablemente se puede hacer mejor)
    public float speed;
}

[System.Serializable]
public class CharacterModel
{
    public string name;
    public string clip;
}