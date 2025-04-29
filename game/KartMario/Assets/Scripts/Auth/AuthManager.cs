using System.Threading.Tasks;
using Injecta;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public bool isLogged = false;
    public bool isTryingToLog = true;
    public static string token = "";

    [Inject]
    private WebsocketSingleton websocket;

    async void Start()
    {
        // Intentar cargar el token guardado
        if (PlayerPrefs.HasKey("AccessToken"))
        {
            token = PlayerPrefs.GetString("AccessToken");
            Debug.Log("Token encontrado en PlayerPrefs: " + token);
            Debug.Log("Usuario autenticado autom�ticamente >:)");

            //StartCoroutine(ConnectToSocketCoroutine(savedToken));
            bool couldSign = await GetUserAsync(PlayerPrefs.GetInt("ID"), token);

            if (!couldSign)
            {
                isTryingToLog = false;
                Logout();
                return;
            }

            isLogged = true;

            await websocket.ConnectToSocket(token);
        }
        else
        {
            isLogged = false;
            Debug.Log("No se ha encontrado el token. Debes iniciar sesi�n. Porfi inicia sesi�n. :c");
        }

        isTryingToLog = false;
    }

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

                token = response.accessToken;

                //StartCoroutine(ConnectToSocketCoroutine(response.accessToken));
                await GetUserAsync(response.id, response.accessToken);
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
            LobbyManager.PlayerId = id;

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
            isLogged = false;
            token = "";
        }
        else
        {
            Debug.Log("No se encontr� ning�n token. No hay sesi�n para cerrar :c");
        }
    }
}
