using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; }
    
    public static readonly string BASE_URL = "https://localhost:7048/";
    public static readonly string API_URL = "https://localhost:7048/api/";
    public static readonly string SOCKET_URL = "wss://localhost:7048/socket/";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para que no se destruya
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
