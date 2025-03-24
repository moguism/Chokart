using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class NetworkingManager : MonoBehaviour
{
    public static NetworkingManager instance;

    // Registro
    public InputField emailInput;
    public InputField nicknameInput;
    public InputField passwordInput;

    // Inicio de sesi�n
    public InputField loginEmailOrNicknameInput;
    public InputField loginPasswordInput;
    public Toggle rememberMeToggle; // Recu�rdame


    void Start()
    {
        instance = this;

        // Intentar cargar el token guardado

        if (PlayerPrefs.HasKey("AccessToken"))
        {
            string savedToken = PlayerPrefs.GetString("AccessToken");
            Debug.Log("Token encontrado en PlayerPrefs: " + savedToken);
            Debug.Log("Usuario autenticado autom�ticamente >:)");
        }
        else
        {
            Debug.Log("No se ha encontrado el token. Debes iniciar sesi�n. Porfi inicia sesi�n. :c");
        }
    }

    void Update() { }

    public void OnRegButton()
    {
        string email = emailInput.text.Trim();
        string nickname = nicknameInput.text.Trim();
        string password = passwordInput.text;

        //Debug.Log("Email introducido: " + email);
        //Debug.Log("Apodo introducido: " + nickname);
        //Debug.Log("Contrase�a introducida: " + password);

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("El email o la contrase�a est�n vac�os.");
            return;
        }

        RegisterRequest tempReg = new RegisterRequest()
        {
            Email = emailInput.text.Trim(),
            Nickname = nicknameInput.text.Trim(),
            Password = passwordInput.text
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

        StartCoroutine(Login(tempLog));
    }

    // Registro
    public IEnumerator Register(RegisterRequest register)
    {
        // Hace la petici�n, convierte los datos del registro a JSON y posteriormente a bytes y lo env�a.
        UnityWebRequest unityWebRequest = new UnityWebRequest(Singleton.API_URL + "Auth/register", "POST");
        string jsonData = JsonUtility.ToJson(register);
        Debug.Log("Datos registro: " + jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro exitoso: " + unityWebRequest.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error en la solicitud: " + unityWebRequest.error);
            Debug.LogError("Respuesta del servidor: " + unityWebRequest.downloadHandler.text);
        }
    }

    // Inicio de sesi�n
    public IEnumerator Login(LoginRequest loginRequest)
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
            Debug.Log("Inicio de sesi�n exitoso: " + unityWebRequest.downloadHandler.text);
            string jsonResponse = unityWebRequest.downloadHandler.text;

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse); // Obtiene �nicamente el token del JSON response
            Debug.Log("Token extra�do: " + response.accessToken);

            // Guardar el token en PlayerPrefs si el "recu�rdame" est� activado
            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetString("AccessToken", response.accessToken);
                PlayerPrefs.Save();
                Debug.Log("Token guardado en PlayerPrefs.");
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

    // Cerrar sesi�n
    public void Logout()
    {
        if (PlayerPrefs.HasKey("AccessToken"))
        {
            PlayerPrefs.DeleteKey("AccessToken");
            Debug.Log("A chuparla el token >:) ; Sesi�n cerrada.");
        }
        else
        {
            Debug.Log("No se encontr� ning�n token. No hay sesi�n para cerrar :c");
        }
    }


}
