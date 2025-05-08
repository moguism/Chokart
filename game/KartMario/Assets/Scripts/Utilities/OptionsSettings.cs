using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsSettings : MonoBehaviour
{
    [SerializeField]
    private Toggle voiceChatToggle;

    [SerializeField]
    private GameObject startGameButton;

    public static bool shouldEnableStartButton = true;

    // La idea es que se como el REMATCH, por ejemplo, que tienes la opción de habilitar o deshabilitar el chat de voz en los ajustes
    public static bool shouldRecord = false;

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
        if(shouldEnableStartButton)
        {
            startGameButton.SetActive(true);
        }
    }
}
