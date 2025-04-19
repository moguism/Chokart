using Injecta;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Inject]
    private AuthManager authManager;

    public async void Login()
    {
        bool couldLogin = await authManager.LoginAsync(emailOrNickname.text, password.text, rememberMe.isOn);
        if(!couldLogin)
        {
            errorMessage.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

}
