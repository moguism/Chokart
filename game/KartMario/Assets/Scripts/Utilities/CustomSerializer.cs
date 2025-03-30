using Injecta;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class CustomSerializer
{
    [Inject]
    private static WebsocketSingleton singleton;

    public static async Task<string> Serialize(Dictionary<object, object> dict, bool send)
    {
        var json = JsonConvert.SerializeObject(dict);
        if(send)
        {
            await singleton.webSocket.SendText(json);
        }
        return json;
    }
}
