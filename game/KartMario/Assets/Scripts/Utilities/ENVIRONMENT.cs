public static class ENVIRONMENT
{
    public const string BASE_URL = "https://chokart.tryasp.net/";
    //public const string BASE_URL = "https://localhost:7048/";
    public const string API_URL = BASE_URL + "api/";
    public static readonly string SOCKET_URL = BASE_URL.Replace("https", "wss") + "socket";
}
