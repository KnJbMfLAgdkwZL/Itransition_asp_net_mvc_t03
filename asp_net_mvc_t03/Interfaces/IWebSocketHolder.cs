using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace asp_net_mvc_t03.Interfaces;

public interface IWebSocketHolder
{
    void AddSocket(WebSocket webSocket, string? userEmail);
    ConcurrentBag<WebSocket>? GetAllSocket(string? userEmail);
    void Remove(WebSocket webSocket, string? userEmail);
    void CheckWebSockets();
    void ClearAndCloseWebSockets(string? userEmail);
}