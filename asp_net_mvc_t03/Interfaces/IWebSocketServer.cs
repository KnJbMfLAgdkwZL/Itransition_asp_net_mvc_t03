namespace asp_net_mvc_t03.Interfaces;

public interface IWebSocketServer
{
    Task AcceptWebSocketAsync(HttpContext context);
    Task EchoAsync(HttpContext context);
    Task SendAsync(object response);
    Task CaseAsync(string jsonRequest, HttpContext httpContext);
}