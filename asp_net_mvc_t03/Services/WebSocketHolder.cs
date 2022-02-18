using System.Collections.Concurrent;
using System.Net.WebSockets;
using asp_net_mvc_t03.Interfaces;

namespace asp_net_mvc_t03.Services;

public class WebSocketHolder : IWebSocketHolder
{
    private ConcurrentDictionary<string?, ConcurrentBag<WebSocket>> AllConnection { set; get; } = new();

    public void AddSocket(WebSocket webSocket, string? userEmail)
    {
        if (!AllConnection.ContainsKey(userEmail))
        {
            AllConnection.TryAdd(userEmail, new ConcurrentBag<WebSocket>());
        }

        if (!AllConnection[userEmail].Contains(webSocket))
        {
            AllConnection[userEmail].Add(webSocket);
        }
        else
        {
            Console.WriteLine($"{userEmail} already in list");
        }

        ShowLog();
    }

    public ConcurrentBag<WebSocket>? GetAllSocket(string? userEmail)
    {
        return AllConnection.ContainsKey(userEmail) ? AllConnection[userEmail] : null;
    }

    public void Remove(WebSocket webSocket, string? userEmail)
    {
        if (AllConnection.ContainsKey(userEmail))
        {
            AllConnection[userEmail].TryTake(out webSocket!);
            if (AllConnection[userEmail].IsEmpty)
            {
                AllConnection.TryRemove(userEmail, out _);
            }
        }

        ShowLog();
    }

    public void CheckWebSockets()
    {
        foreach (var (key, value) in AllConnection)
        {
            foreach (var socket in value)
            {
                if (socket.State != WebSocketState.Connecting && socket.State != WebSocketState.Open)
                {
                    socket.CloseAsync(WebSocketCloseStatus.Empty, "CheckWebSockets close", CancellationToken.None);
                    Remove(socket, key);
                }
            }
        }
    }

    public void ClearAndCloseWebSockets(string? userEmail)
    {
        if (!AllConnection.ContainsKey(userEmail))
        {
            return;
        }

        foreach (var val in AllConnection[userEmail])
        {
            if (val.State != WebSocketState.Closed)
            {
                val.CloseAsync(WebSocketCloseStatus.Empty, "Closed by server", CancellationToken.None);
            }
        }

        AllConnection[userEmail].Clear();
        AllConnection.TryRemove(userEmail, out _);

        ShowLog();
    }

    private void ShowLog()
    {
        Console.WriteLine();
        foreach (var (key, value) in AllConnection)
        {
            Console.WriteLine($"Connection: {key} = {value.Count}");
        }

        var total = AllConnection.Sum(pair => pair.Value.Count);
        Console.WriteLine($"Total: {total}");
        Console.WriteLine();
    }
}