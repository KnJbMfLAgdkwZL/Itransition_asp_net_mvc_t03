using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_mvc_t03.Controllers;

public class WebSocketController : ControllerBase
{
    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(HttpContext, webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }

    private async Task Echo(HttpContext context, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(buffer,
                    0,
                    result.Count),
                result.MessageType,
                result.EndOfMessage,
                CancellationToken.None);

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}