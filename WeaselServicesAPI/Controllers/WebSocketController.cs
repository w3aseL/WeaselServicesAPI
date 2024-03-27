using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebSocketService;

namespace WeaselServicesAPI.Controllers
{
    [EnableCors("DashboardPolicy")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly ConnectionManager _connectionManager;
        private readonly ILogger<WebSocketController> _logger;

        public WebSocketController(ConnectionManager connManager, ILogger<WebSocketController> logger)
        {
            _connectionManager = connManager;
            _logger = logger;
        }

        [HttpGet, Route("/ws")]
        public async Task ProcessWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var guid = _connectionManager.AddConnection(webSocket);

                _logger.Log(LogLevel.Information, $"New connection made, identifier \"{guid}\" assigned.");

                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    var result = _connectionManager.ProcessWebSocketResult(receiveResult, buffer);

                    await _connectionManager.GetEventManager().ProcessEvent(result, webSocket, _logger);

                    receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(
                    receiveResult.CloseStatus.Value,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);

                _connectionManager.RemoveConnection(guid);
                _logger.Log(LogLevel.Information, $"Connection with identifier \"{guid}\" closed.");
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
