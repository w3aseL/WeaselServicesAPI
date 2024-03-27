using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketService;

public class SocketRequest
{
    public string Event { get; set; }
    public JValue Data { get; set; }
}

public class SocketResponse
{
    [JsonProperty("event")]
    public string Event { get; set; }
    [JsonProperty("data")]
    public dynamic Data { get; set; }
}
