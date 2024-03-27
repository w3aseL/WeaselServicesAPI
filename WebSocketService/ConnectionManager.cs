using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WebSocketService;

public class ConnectionManager
{
    private EventManager _eventManager;
    private ConcurrentDictionary<Guid, WebSocket> Sockets { get; }
    private List<SubscriptionRecord> Subscriptions { get; }

    public ConnectionManager()
    {
        _eventManager = new EventManager();
        Sockets = new ConcurrentDictionary<Guid, WebSocket>();
        Subscriptions = new List<SubscriptionRecord>();

        _eventManager.AddEvent("subscribe", SubscribeEvent);
        _eventManager.AddEvent("unsubscribe", UnsubscribeEvent);
        _eventManager.AddEvent("subscription-list", SubscriptionListEvent);
    }

    public Guid AddConnection(WebSocket socket)
    {
        var tries = 10;

        while (tries > 0)
        {
            var newGuid = Guid.NewGuid();

            if (Sockets.TryAdd(newGuid, socket))
                return newGuid;
        }

        throw new Exception("Failed to add socket to connection manager! Too many tries to add to dictionary.");
    }

    private async Task SubscribeEvent(WebSocket socket, JValue data, ILogger logger)
    {
        string eventName = data.ToObject<string>();

        var response = new SocketResponse
        {
            Event = "message",
            Data = $"Failed to subscribe to event \"${eventName}\"!"
        };

        var guid = Sockets.FirstOrDefault(s => s.Value == socket).Key;

        if (SubscribeConnection(guid, eventName)) response.Data = $"Successfully subscribed to event \"{eventName}\"!";

        await socket.SendAsync(
            _eventManager.ConvertTextToArraySegment(JsonConvert.SerializeObject(response)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    private async Task UnsubscribeEvent(WebSocket socket, JValue data, ILogger logger)
    {
        string eventName = data.ToObject<string>();

        var response = new SocketResponse
        {
            Event = "message",
            Data = $"Failed to unsubscribe from event \"${eventName}\"!"
        };

        var guid = Sockets.FirstOrDefault(s => s.Value == socket).Key;

        if (UnsubscribeConnection(guid, eventName)) response.Data = $"Successfully unsubscribed from event \"{eventName}\"!";

        await socket.SendAsync(
            _eventManager.ConvertTextToArraySegment(JsonConvert.SerializeObject(response)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    private async Task SubscriptionListEvent(WebSocket socket, JValue _, ILogger logger)
    {
        var guid = Sockets.FirstOrDefault(s => s.Value == socket).Key;
        var subscribedEvents = Subscriptions.Where(s => s.Guid == guid).Select(s => s.EventName).ToList();

        var response = new SocketResponse
        {
            Event = "data",
            Data = subscribedEvents.ToArray()
        };

        await socket.SendAsync(
            _eventManager.ConvertTextToArraySegment(JsonConvert.SerializeObject(response)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    public async Task<bool> EmitEvent(string eventName, dynamic data)
    {
        var guids = Subscriptions.Where(s => s.EventName == eventName).Select(s => s.Guid).ToList();
        if (guids.Count == 0) return false;

        var sockets = Sockets.Where(s => guids.Contains(s.Key)).Select(s => s.Value).ToList();

        var subscriptionResponse = new SocketResponse
        {
            Event = eventName,
            Data = data
        };

        foreach (var socket in sockets)
        {
            await socket.SendAsync(
                _eventManager.ConvertTextToArraySegment(JsonConvert.SerializeObject(subscriptionResponse)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        return true;
    }

    private bool SubscribeConnection(Guid guid, string e)
    {
        if (!Subscriptions.Any(s => s.Guid == guid && s.EventName == e))
            lock (Subscriptions)
            {
                Subscriptions.Add(new SubscriptionRecord { Guid = guid, EventName = e });
                return true;
            }

        return false;
    }

    private bool UnsubscribeConnection(Guid guid, string e)
    {
        var sub = Subscriptions.FirstOrDefault(s => s.Guid == guid && s.EventName == e);
        if (sub is null) return false;

        lock (Subscriptions)
        {
            Subscriptions.Remove(sub);
        }

        return true;
    }

    public bool RemoveConnection(Guid guid)
    {
        // Remove subscriptions
        var subs = Subscriptions.Where(s => s.Guid == guid).ToList();
        if (subs.Count > 0)
        {
            lock (Subscriptions)
            {
                foreach (var sub in subs)
                    Subscriptions.Remove(sub);
            }
        }

        return Sockets.TryRemove(guid, out _);
    }

    public EventManager GetEventManager()
    {
        return _eventManager;
    }

    public SocketRequest ProcessWebSocketResult(WebSocketReceiveResult result, byte[] buffer)
    {
        var jsonStr = Encoding.UTF8.GetString(buffer, 0, result.Count);

        return JsonConvert.DeserializeObject<SocketRequest>(jsonStr);
    }

    private class SubscriptionRecord
    {
        public Guid Guid { get; set; }
        public string EventName { get; set; }
    }
}
