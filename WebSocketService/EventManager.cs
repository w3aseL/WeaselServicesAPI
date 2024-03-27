using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WebSocketService;

public class EventManager
{
    private Dictionary<string, Func<WebSocket, JValue, ILogger, Task>> _events { get; }

    public EventManager()
    {
        _events = new Dictionary<string, Func<WebSocket, JValue, ILogger, Task>>();

        // TODO: Declare events here
        _events.Add("test", TestEvent);
    }

    public async Task ProcessEvent(SocketRequest request, WebSocket socket, ILogger logger)
    {
        if (_events.TryGetValue(request.Event, out Func<WebSocket, JValue, ILogger, Task> func))
        {
            await func(socket, request.Data, logger);
            return;
        }

        throw new Exception($"Could not find event function with name \"{request.Event}\"");
    }

    public void AddEvent(string eventName, Func<WebSocket, JValue, ILogger, Task> func)
    {
        _events.Add(eventName, func);
    }

    private async Task TestEvent(WebSocket socket, JValue data, ILogger logger)
    {
        var serializedData = data.ToObject<TestData>();

        logger.Log(LogLevel.Information, $"Received message: \"{serializedData.Message}\"");

        var response = new SocketResponse
        {
            Event = "message",
            Data = "Received test message!"
        };

        await socket.SendAsync(
            ConvertTextToArraySegment(JsonConvert.SerializeObject(response)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    public ArraySegment<byte> ConvertTextToArraySegment(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        return new ArraySegment<byte>(bytes, 0, bytes.Length);
    }

    private class TestData
    {
        public string Message { get; set; }
    }
}
