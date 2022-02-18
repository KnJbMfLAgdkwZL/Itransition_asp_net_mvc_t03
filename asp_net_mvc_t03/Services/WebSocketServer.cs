using System.Net.WebSockets;
using System.Text;
using asp_net_mvc_t03.DTO;
using asp_net_mvc_t03.Enums;
using asp_net_mvc_t03.Interfaces;
using asp_net_mvc_t03.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace asp_net_mvc_t03.Services;

public class WebSocketServer : IWebSocketServer
{
    private WebSocket? WebSocket { set; get; }
    private readonly MasterContext _masterContext;
    private string? _uName = null;
    private IWebSocketHolder WebSocketHolder { set; get; }

    private delegate Task CaseHandler(dynamic request, HttpContext httpContext);

    private readonly Dictionary<string, CaseHandler> _cases;

    public WebSocketServer(MasterContext masterContext, IWebSocketHolder webSocketHolder)
    {
        _masterContext = masterContext;
        WebSocketHolder = webSocketHolder;

        _cases = new Dictionary<string, CaseHandler>
        {
            {"GetUsersEmail", GetUsersEmailAsync},
            {"CreateMessage", CreateMessageAsync},
            {"GetTopics", GetTopicsAsync},
            {"GetMessages", GetMessagesAsync}
        };
    }

    public async Task AcceptWebSocketAsync(HttpContext httpContext)
    {
        WebSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
        _uName = httpContext!.User.Identity?.Name;
        if (_uName != null)
        {
            WebSocketHolder.AddSocket(WebSocket, _uName);
        }
        else
        {
            await WebSocket.CloseAsync(WebSocketCloseStatus.Empty, "Closed by server Identity null",
                CancellationToken.None);
        }
    }

    public async Task EchoAsync(HttpContext httpContext)
    {
        var token = CancellationToken.None;

        const int bufferSize = 1024 * 4;
        var buffer = new byte[bufferSize];
        var result = await WebSocket!.ReceiveAsync(new ArraySegment<byte>(buffer), token);
        while (!result.CloseStatus.HasValue)
        {
            var jsonRequest = Encoding.Default.GetString(buffer);

            Console.WriteLine(jsonRequest);

            await CaseAsync(jsonRequest, httpContext!);
            Array.Clear(buffer, 0, buffer.Length);

            result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
        }

        await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, token);
        WebSocketHolder.Remove(WebSocket, _uName);
    }

    public async Task SendAsync(object response)
    {
        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        var jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented, settings);
        var bytes = Encoding.ASCII.GetBytes(jsonResponse);
        await WebSocket!.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
            WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
    }

    public async Task SendAsync(object response, WebSocket socket)
    {
        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        var jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented, settings);
        var bytes = Encoding.ASCII.GetBytes(jsonResponse);
        await socket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
            WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
    }

    public async Task CaseAsync(string jsonRequest, HttpContext httpContext)
    {
        var request = JsonConvert.DeserializeObject<dynamic>(jsonRequest);
        if (request != null)
        {
            var type = request.Type.ToString();
            if (_cases.ContainsKey(type))
            {
                await _cases[type](request, httpContext);
            }
            else
            {
                await SendAsync(new WebSocketMessage()
                {
                    Type = type + "Response",
                    Error = "Wrong Operation"
                });
            }
        }
    }

    private async Task GetUsersEmailAsync(dynamic request, HttpContext httpContext)
    {
        var token = CancellationToken.None;

        string search = request.Data.Search;

        var listString = await _masterContext.Users
            .Where(user =>
                EF.Functions.Like(user.Email, $"%{search}%") &&
                user.Status == UserStatus.Unblock.ToString())
            .OrderBy(user => user.Email)
            .Take(5)
            .Select(user => user.Email)
            .ToListAsync(token);

        var response = new WebSocketMessage
        {
            Type = request.Type + "Response",
            Data = listString
        };

        await SendAsync(response);
    }

    private async Task CreateMessageAsync(dynamic request, HttpContext httpContext)
    {
        var response = new WebSocketMessage
        {
            Type = request.Type + "Response",
        };

        var token = CancellationToken.None;

        await using var transaction = await _masterContext.Database.BeginTransactionAsync(token);

        var author = await _masterContext.Users
            .Where(user => user.Email == _uName && user.Status == UserStatus.Unblock.ToString())
            .FirstOrDefaultAsync(token);

        var toUserName = (string) request.Data.ToUser.ToString();
        var toUser = await _masterContext.Users
            .Where(user => user.Email == toUserName)
            .FirstOrDefaultAsync(token);

        if (author == null || toUser == null)
        {
            response.Error = "Author or ToUser not found";
            await SendAsync(response);
            return;
        }

        var modelMessage = new Message()
        {
            Head = CutStr(request.Data.Head.ToString(), 100),
            Body = CutStr(request.Data.Body.ToString(), 1000),
            AuthorId = author.Id,
            ToUserId = toUser.Id,
            CreateDate = DateTime.Now,
            New = true,
            ReplyId = null
        };
        modelMessage.Uid = GetHashString(author.Id + toUser.Id + modelMessage.Head);

        var entityEntry = await _masterContext.Messages.AddAsync(modelMessage, token);
        await _masterContext.SaveChangesAsync(token);
        response.Data = entityEntry.Entity;

        await transaction.CommitAsync(token);
        await SendAsync(response);

        await RefreshTopics(toUser.Email);
        await RefreshDialog(toUser.Email, modelMessage.Uid);

        await RefreshTopics(author.Email);
    }

    private async Task GetTopicsAsync(dynamic request, HttpContext httpContext)
    {
        var response = new WebSocketMessage
        {
            Type = request.Type + "Response",
        };

        var token = CancellationToken.None;

        var curUser = await _masterContext.Users
            .Where(u =>
                u.Email == _uName &&
                u.Status == UserStatus.Unblock.ToString())
            .FirstOrDefaultAsync(token);

        if (curUser == null)
        {
            response.Error = "Current User not found";
            await SendAsync(response);
            return;
        }

        var topics = _masterContext.Messages
            .Where(message =>
                message.AuthorId == curUser.Id ||
                message.ToUserId == curUser.Id)
            .Include(m => m.Author)
            .Include(m => m.ToUser)
            .AsEnumerable()
            .OrderByDescending(m => m.CreateDate)
            .GroupBy(m => m.Uid)
            .Select(m => m.First())
            .Take(10)
            .ToList();

        response.Data = topics;
        await SendAsync(response);
    }

    private async Task GetMessagesAsync(dynamic request, HttpContext httpContext)
    {
        var response = new WebSocketMessage
        {
            Type = request.Type + "Response",
        };

        var token = CancellationToken.None;

        var curUser = await _masterContext.Users
            .Where(u =>
                u.Email == _uName &&
                u.Status == UserStatus.Unblock.ToString())
            .FirstOrDefaultAsync(token);
        if (curUser == null)
        {
            response.Error = "Current User not found";
            await SendAsync(response);
            return;
        }

        string uid = request.Data.Uid;
        var messages = await _masterContext.Messages
            .Where(m =>
                (m.AuthorId == curUser.Id || m.ToUserId == curUser.Id) &&
                m.Uid == uid
            )
            .Include(m => m.Author)
            .Include(m => m.ToUser)
            .OrderBy(m => m.CreateDate)
            .Take(100)
            .ToListAsync(token);

        response.Data = new
        {
            Messages = messages,
            UserId = curUser.Id
        };
        await SendAsync(response);
    }

    private string GetHashString(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        using var sha = new System.Security.Cryptography.SHA256Managed();
        var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        var hashBytes = sha.ComputeHash(textBytes);

        var hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", string.Empty);
        return hash;
    }

    private string CutStr(string str, int maxSize = 10000)
    {
        str = str.Trim();
        if (str.Length >= maxSize)
        {
            str = str.Substring(0, maxSize);
        }

        return str;
    }

    private async Task RefreshTopics(string email)
    {
        var sockets = WebSocketHolder.GetAllSocket(email);
        if (sockets == null)
        {
            return;
        }

        var response = new WebSocketMessage
        {
            Type = "RefreshTopics",
            Data = null,
            Error = ""
        };
        foreach (var v in sockets)
        {
            await SendAsync(response, v);
        }
    }

    private async Task RefreshDialog(string email, string uid)
    {
        var sockets = WebSocketHolder.GetAllSocket(email);
        if (sockets == null)
        {
            return;
        }

        var response = new WebSocketMessage
        {
            Type = "RefreshDialog",
            Data = new
            {
                Uid = uid
            },
            Error = ""
        };
        foreach (var v in sockets)
        {
            await SendAsync(response, v);
        }
    }
}