using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    public bool isLogged = false;
    public bool isTryingToLog = true;

    async void Start()
    {
        // Intentar cargar el token guardado
        if (PlayerPrefs.HasKey("AccessToken"))
        {
            string savedToken = PlayerPrefs.GetString("AccessToken");
            Debug.Log("Token encontrado en PlayerPrefs: " + savedToken);
            Debug.Log("Usuario autenticado autom�ticamente >:)");

            //StartCoroutine(ConnectToSocketCoroutine(savedToken));
            bool couldSign = await GetUserAsync(PlayerPrefs.GetInt("ID"), savedToken);

            if (!couldSign)
            {
                isLogged = false;
                isTryingToLog = false;
                Logout();
                return;
            }

            isLogged = true;
        }
        else
        {
            isLogged = false;
            Debug.Log("No se ha encontrado el token. Debes iniciar sesi�n. Porfi inicia sesi�n. :c");
        }

        isTryingToLog = false;
    }

    /*public async void OnRegButton(string email, string nickname, string password)
    {

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("El email o la contrase�a est�n vac�os.");
            return;
        }

        RegisterRequest tempReg = new RegisterRequest()
        {
            Email = email,
            Nickname = nickname,
            Password = password
        };

        await Register(tempReg);
    }*/

    /*public async void OnLogInButton()
    {
        string emailOrNickname = loginEmailOrNicknameInput.text.Trim();
        string password = loginPasswordInput.text;

        LoginRequest tempLog = new LoginRequest()
        {
            EmailOrNickname = emailOrNickname,
            Password = password
        };

        await LoginAsync(tempLog);
    }*/

    // Registro
    /*public async Task Register(string email, string nickname, string password)
    {

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("El email o la contrase�a est�n vac�os.");
            return;
        }

        RegisterRequest register = new RegisterRequest()
        {
            Email = email,
            Nickname = nickname,
            Password = password
        };

        // Hace la petici�n, convierte los datos del registro a JSON y posteriormente a bytes y lo env�a.
        UnityWebRequest unityWebRequest = new UnityWebRequest(ENVIRONMENT.API_URL + "Auth/register", "POST");
        string jsonData = JsonUtility.ToJson(register);
        Debug.Log("Datos registro: " + jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json"); // Este Content-Type no sirve para cuando subamos avatares

        await unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro exitoso: " + unityWebRequest.downloadHandler.text);
            await LoginAsync(register.Nickname, register.Password, false);
        }
        else
        {
            Debug.LogError("Error en la solicitud: " + unityWebRequest.error);
            Debug.LogError("Respuesta del servidor: " + unityWebRequest.downloadHandler.text);
        }
    }*/

    // Inicio de sesi�n
    public async Task<bool> LoginAsync(string emailOrNickname, string password, bool rememberMe)
    {
        if(emailOrNickname == "" || emailOrNickname == null || password == "" || password == null)
        {
            return false;
        }

        LoginRequest loginRequest = new LoginRequest()
        {
            EmailOrNickname = emailOrNickname,
            Password = password
        };


        UnityWebRequest unityWebRequest = new UnityWebRequest(ENVIRONMENT.API_URL + "Auth/login", "POST");
        string jsonData = JsonUtility.ToJson(loginRequest);
        Debug.Log("Datos login: " + jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");

        await unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Inicio de sesi�n exitoso: " + unityWebRequest.downloadHandler.text);
            string jsonResponse = unityWebRequest.downloadHandler.text;

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse); // Obtiene �nicamente el token del JSON response
            Debug.Log("Token extra�do: " + response.accessToken);

            // Guardar el token en PlayerPrefs si el "recu�rdame" est� activado
            if (rememberMe)
            {
                PlayerPrefs.SetInt("ID", response.id);
                PlayerPrefs.SetString("AccessToken", response.accessToken);
                PlayerPrefs.Save();
                Debug.Log("Token guardado en PlayerPrefs.");

                //StartCoroutine(ConnectToSocketCoroutine(response.accessToken));
                await GetUserAsync(response.id, response.accessToken);

                SceneManager.LoadScene(2); // La pantalla de lobbies
            }
            else
            {
                Debug.Log("El token no se ha guardado en PlayerPrefs.");
            }

            //SaveTokenToFile(response.accessToken); // Guarda el token en un txt

            return true;
        }
        else
        {
            Debug.LogError("Error en la solicitud: " + unityWebRequest.error);
            Debug.LogError("Respuesta del servidor: " + unityWebRequest.downloadHandler.text);
            return false;
        }
    }

    public async Task<bool> GetUserAsync(int id, string token)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(ENVIRONMENT.API_URL + "User/" + id, "GET");

        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + token);

        await unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = unityWebRequest.downloadHandler.text;

            UserResponse response = JsonUtility.FromJson<UserResponse>(jsonResponse);
            PlayerPrefs.SetString("PlayerName", response.nickname);
            PlayerPrefs.Save();

            // TODO: Mostrar un aviso si está baneado
            LobbyManager.PlayerName = response.nickname;

            return true;
        }
        else
        {
            return false;
        }
    }

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
