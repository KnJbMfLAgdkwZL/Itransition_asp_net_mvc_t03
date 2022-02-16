using System.Net;
using asp_net_mvc_t03.Interfaces;
using asp_net_mvc_t03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_mvc_t03.Controllers;

[Authorize]
public class WebSocketController : ControllerBase
{
    private readonly MasterContext _masterContext;
    private readonly IWebSocketServer _webSocketServer;

    public WebSocketController(MasterContext masterContext, IWebSocketServer webSocketServer)
    {
        _masterContext = masterContext;
        _webSocketServer = webSocketServer;
    }

    [HttpGet("/ws")]
    public async Task GetAsync()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            await _webSocketServer.AcceptWebSocketAsync(HttpContext);
            await _webSocketServer.EchoAsync(HttpContext);
        }
        else
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }
}