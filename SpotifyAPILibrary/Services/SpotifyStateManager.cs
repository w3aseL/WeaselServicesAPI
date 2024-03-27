using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketService;

namespace SpotifyAPILibrary
{
    public class SpotifyStateManager
    {
        private ILogger<SpotifyStateManager> _logger;
        private Dictionary<int, SpotifyPlayerActiveState> activeStates;
        private ReaderWriterLockSlim _lock;
        private SpotifySessionJobQueue _queue;
        private ConnectionManager _connectionManager;

        public SpotifyStateManager(ILogger<SpotifyStateManager> logger, SpotifySessionJobQueue queue, ConnectionManager manager)
        {
            _logger = logger;
            activeStates = new Dictionary<int, SpotifyPlayerActiveState>();
            _lock = new ReaderWriterLockSlim();
            _queue = queue;
            _connectionManager = manager;

            // events
            _connectionManager.GetEventManager().AddEvent("player-status", GetCurrentStateEvent);
        }

        ~SpotifyStateManager()
        {
            if(_lock != null) _lock.Dispose();
        }

        public SpotifyPlayerActiveState GetActiveState(int accountId)
        {
            _lock.EnterReadLock();

            try
            {
                return activeStates[accountId].ShallowClone();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public (bool, string) UpdateActiveState(int accountId, CurrentlyPlayingContext? ctx)
        {
            _lock.EnterWriteLock();

            try
            {
                SpotifyPlayerActiveState state;
                var hasKey = activeStates.TryGetValue(accountId, out state);

                if (!hasKey)
                {
                    state = new SpotifyPlayerActiveState(accountId, _queue);
                    activeStates.Add(accountId, state);
                }

                return state.UpdateActiveState(ctx);
            }
            finally { _lock.ExitWriteLock(); }
        }

        #region State Events
        
        public async Task GetCurrentStateEvent(WebSocket socket, JValue data, ILogger logger)
        {
            int? accountId = data.ToObject<int?>();

            if (!accountId.HasValue)
            {
                await SendSocketData(socket, "message", "An \"accountId\" was not provided. Could not find the player state! ");
                return;
            }

            var currentState = GetActiveState(accountId.Value);

            if (currentState is null)
            {
                await SendSocketData(socket, "message", $"Could not find a player state with the \"acccountId\" of {accountId}!");
                return;
            }

            await SendSocketData(socket, "data", new { eventName = "player-status", data = currentState });
            return;
        }

        private async Task SendSocketData(WebSocket socket, string eventName, dynamic data)
        {
            var res = new SocketResponse
            {
                Event = eventName,
                Data = data
            };

            await socket.SendAsync(
                _connectionManager.GetEventManager().ConvertTextToArraySegment(JsonConvert.SerializeObject(res)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        #endregion
    }
}
