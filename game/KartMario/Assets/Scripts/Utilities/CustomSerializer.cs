using Injecta;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CustomSerializer
{
    private readonly WebsocketSingleton _singleton;

    public CustomSerializer(WebsocketSingleton singleton)
    {
        _singleton = singleton;
    }

    public async Task<string> Serialize(Dictionary<object, object> dict, bool send)
    {
        var json = JsonConvert.SerializeObject(dict);
        if(send)
        {
            await _singleton.webSocket.SendText(json);
        }
        return json;
    }
}
