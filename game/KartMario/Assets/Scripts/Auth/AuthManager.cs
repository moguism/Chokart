using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    // Registro
    public InputField emailInput;
    public InputField nicknameInput;
    public InputField passwordInput;

    // Inicio de sesión
    public InputField loginEmailOrNicknameInput;
    public InputField loginPasswordInput;
    public Toggle rememberMeToggle; // Recuérdame

    public GameObject loginPanel;
    public GameObject registerPanel;


    void Start()
    {
        instance = this;

        // Intentar cargar el token guardado

        if (PlayerPrefs.HasKey("AccessToken"))
        {
            string savedToken = PlayerPrefs.GetString("AccessToken");
            Debug.Log("Token encontrado en PlayerPrefs: " + savedToken);
            Debug.Log("Usuario autenticado automáticamente >:)");
            StartCoroutine(ConnectToSocketCoroutine(savedToken));
        }
        else
        {
            Debug.Log("No se ha encontrado el token. Debes iniciar sesión. Porfi inicia sesión. :c");
        }
    }

    public void OnRegButton()
    {
        string email = emailInput.text.Trim();
        string nickname = nicknameInput.text.Trim();
        string password = passwordInput.text;

        //Debug.Log("Email introducido: " + email);
        //Debug.Log("Apodo introducido: " + nickname);
        //Debug.Log("Contraseña introducida: " + password);

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("El email o la contraseña están vacíos.");
            return;
        }

        RegisterRequest tempReg = new RegisterRequest()
        {
            Email = email,
            Nickname = nickname,
            Password = password
        };

        StartCoroutine(Register(tempReg));
    }

    public void OnLogInButton()
    {
        string emailOrNickname = loginEmailOrNicknameInput.text.Trim();
        string password = loginPasswordInput.text;

        LoginRequest tempLog = new LoginRequest()
        {
            EmailOrNickname = emailOrNickname,
            Password = password
        };

        StartCoroutine(LoginAsync(tempLog));
    }

    // Registro
    public IEnumerator Register(RegisterRequest register)
    {
        // Hace la petición, convierte los datos del registro a JSON y posteriormente a bytes y lo envía.
        UnityWebRequest unityWebRequest = new UnityWebRequest(Singleton.API_URL + "Auth/register", "POST");
        string jsonData = JsonUtility.ToJson(register);
        Debug.Log("Datos registro: " + jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json"); // Este Content-Type no sirve para cuando subamos avatares

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro exitoso: " + unityWebRequest.downloadHandler.text);
            StartCoroutine(
                LoginAsync(new LoginRequest()
                {
                    EmailOrNickname = register.Nickname,
                    Password = register.Password
                }
            ));
        }
        else
        {
            Debug.LogError("Error en la solicitud: " + unityWebRequest.error);
            Debug.LogError("Respuesta del servidor: " + unityWebRequest.downloadHandler.text);
        }
    }

    // Inicio de sesión
    public IEnumerator LoginAsync(LoginRequest loginRequest)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(Singleton.API_URL + "Auth/login", "POST");
        string jsonData = JsonUtility.ToJson(loginRequest);
        Debug.Log("Datos login: " + jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Inicio de sesión exitoso: " + unityWebRequest.downloadHandler.text);
            string jsonResponse = unityWebRequest.downloadHandler.text;

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse); // Obtiene únicamente el token del JSON response
            Debug.Log("Token extraído: " + response.accessToken);

            // Guardar el token en PlayerPrefs si el "recuérdame" está activado
            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetString("AccessToken", response.accessToken);
                PlayerPrefs.Save();
                Debug.Log("Token guardado en PlayerPrefs.");

                StartCoroutine(ConnectToSocketCoroutine(response.accessToken));
                SceneManager.LoadScene(1); // Las lobbies
            }
            else
            {
                Debug.Log("El token no se ha guardado en PlayerPrefs.");
            }

            //SaveTokenToFile(response.accessToken); // Guarda el token en un txt
        }
        else
        {
            Debug.LogError("Error en la solicitud: " + unityWebRequest.error);
            Debug.LogError("Respuesta del servidor: " + unityWebRequest.downloadHandler.text);
        }
    }

    //public void SaveTokenToFile(string token)
    //{
    //    string filePath = Application.dataPath + "/AccessToken.txt";
    //    File.WriteAllText(filePath, token);
    //    Debug.Log("Token guardado en: " + filePath); // Por si me desubico xd
    //}

    // Pereza las corrutinas tu xD
    private IEnumerator ConnectToSocketCoroutine(string token)
    {
        Task connectTask = Singleton.Instance.ConnectToSocket(token);
        yield return new WaitUntil(() => connectTask.IsCompleted);

        if (connectTask.IsFaulted)
        {
            SceneManager.LoadScene(0);
            Debug.LogError("Error al conectar al WebSocket :c");
        }
        else
        {
            SceneManager.LoadScene(1);
            Debug.Log("Conexión WebSocket establecida :D");
        }
    }

    // Cerrar sesión
    public void Logout()
    {
        if (PlayerPrefs.HasKey("AccessToken"))
        {
            PlayerPrefs.DeleteKey("AccessToken");
            Debug.Log("A chuparla el token >:) ; Sesión cerrada.");
        }
        else
        {
            Debug.Log("No se encontró ningún token. No hay sesión para cerrar :c");
        }
    }


}
