using System.Linq;
using TMPro;
using UnityEngine;
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
    private VideoPlayer videoPlayer;

    [SerializeField]
    private RawImage backgroundImage;

    [SerializeField]
    private RawImage videoRawImage;
    private Color startVideoColor;

    private int index;
    public static int characterIndex; // En este caso, como el personaje es un cosmético no hace falta guardarlo en el websocket (simplemente se instancia encima del coche)

    private bool showingCharacters = false;

    private void Start()
    {
        cars = _cars.ToArray(); // Para que haga una copia
        characters = _characters.ToArray();

        videoPlayer.SetDirectAudioVolume(0, 0.25f); // Para el volumen
        startVideoColor = videoRawImage.color;

        index = PlayerPrefs.GetInt("carIndex");
        characterIndex = PlayerPrefs.GetInt("characterIndex");
        ManageCarVisibility();
    }

    private void FixedUpdate()
    {
        kartBase.transform.Rotate(0, 1f, 0);
    }

    public void Next()
    {
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
            if(characterIndex >= _characters.Length)
            {
                characterIndex = 0;
            }
        }
        ManageVisibilityAndSave();
    }

    public void Prev()
    {
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
            if(characterIndex < 0)
            {
                characterIndex = _characters.Length - 1;
            }
        }
        ManageVisibilityAndSave();
    }

    public void GoToGame()
    {
        WebsocketSingleton.kartModelIndex = index;

        if (showingCharacters)
        {
            SceneManager.LoadScene(1);   
        }
        else
        {
            showingCharacters = true;
            ManageVisibilityAndSave();
        }
    }

    private void ManageVisibilityAndSave()
    {
        if(showingCharacters)
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
        for(int i = 0; i < _cars.Length; i++)
        {
            _cars[i].car.SetActive(false);
        }

        _cars[index].car.SetActive(true);
        speedText.text = "Speed: " + _cars[index].speed;
    }

    private void ManageCharacterVisibility()
    {
        _cars[index].car.GetComponentInChildren<CharacterSelector>().SetCharacter(characterIndex, false);

        speedText.text = _characters[characterIndex].name;

        VideoClip clip = _characters[characterIndex].clip;
        if(clip == null)
        {
            videoPlayer.Stop();
            Debug.LogWarning("No hay video :(");
            SetVideoAndImageAvailability(true, false);
            return;
        }

        SetVideoAndImageAvailability(false, true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
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
    public VideoClip clip;
}