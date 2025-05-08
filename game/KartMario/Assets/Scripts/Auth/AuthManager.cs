using Injecta;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public bool isLogged = false;
    public bool isTryingToLog = true;
    public static string token = "";

    public static UserDto user;
    public static SteamProfile steamProfile;

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
            bool couldSign = await GetUserAsync(PlayerPrefs.GetInt("ID"));

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
        if (emailOrNickname == "" || emailOrNickname == null || password == "" || password == null)
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
                await GetUserAsync(response.id);
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

    public async Task<bool> GetUserAsync(int id)
    {
        UnityWebRequest unityWebRequest = await PrepareWebRequestAndSendAsync(ENVIRONMENT.API_URL + "User/" + id);

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = unityWebRequest.downloadHandler.text;
            user = JsonUtility.FromJson<UserDto>(jsonResponse);

            if (user == null)
            {
                return false;
            }

            await GetSteamProfile();

            PlayerPrefs.SetString("PlayerName", user.nickname);
            PlayerPrefs.Save();

            // TODO: Mostrar un aviso si está baneado
            LobbyManager.PlayerName = user.nickname;
            LobbyManager.PlayerId = id;

            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task GetSteamProfile()
    {
        try
        {
            if (user.steamId != null && user.steamId != "")
            {
                UnityWebRequest steamRequest = await PrepareWebRequestAndSendAsync(ENVIRONMENT.API_URL + "SteamAuth/getId/" + user.steamId);
                if (steamRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = steamRequest.downloadHandler.text;
                    steamProfile = JsonUtility.FromJson<SteamProfile>(jsonResponse);

                    if(steamProfile == null)
                    {
                        return;
                    }

                    user.nickname = steamProfile.personaName;

                    Debug.Log("Nombre de Steam: " + user.nickname);
                }
            }
        }
        catch { }
    }

    private async Task<UnityWebRequest> PrepareWebRequestAndSendAsync(string path)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(path, "GET");

        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + token);

        await unityWebRequest.SendWebRequest();

        return unityWebRequest;
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
