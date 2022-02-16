namespace asp_net_mvc_t03.DTO;

public class WebSocketMessage
{
    public string Type { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public object? Data { get; set; } = null;
}