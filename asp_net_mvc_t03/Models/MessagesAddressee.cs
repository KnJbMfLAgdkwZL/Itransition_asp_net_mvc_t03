namespace asp_net_mvc_t03.Models;

public partial class MessagesAddressee
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public bool New { get; set; }

    public virtual Message Message { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}