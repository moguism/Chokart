using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsSettings : MonoBehaviour
{
    [SerializeField]
    private Toggle voiceChatToggle;

    public GameObject startGameButton;

    [SerializeField]
    private GameObject resolutionObject;

    public static bool shouldEnableStartButton = true;

    // La idea es que se como el REMATCH, por ejemplo, que tienes la opción de habilitar o deshabilitar el chat de voz en los ajustes
    public static bool shouldRecord = false;

    public static bool showExplosions = true;

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Destroy(resolutionObject);
#endif
        if(PlayerPrefs.HasKey("EXPLOSIONS"))
        {
            string value = PlayerPrefs.GetString("EXPLOSIONS");
            showExplosions = bool.Parse(value);
        }

    }

    public void ManageVoiceChat()
    {
        shouldRecord = voiceChatToggle.isOn;
    }

    public static void ChangeMotorSpeed(float left, float right)
    {
        try
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Gamepad.current.SetMotorSpeeds(left, right);
#endif
        }
        catch { }
    }

    public void ManageAvailability()
    {
        if(startGameButton != null)
        {
            startGameButton.SetActive(true);
        }
    }

    public void ChangeExplosions(bool enable)
    {
        showExplosions = enable;

        PlayerPrefs.SetString("EXPLOSIONS", showExplosions.ToString());
        PlayerPrefs.Save();
    }
}
