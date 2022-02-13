namespace asp_net_mvc_t03.Models;

public partial class Message
{
    public Message()
    {
        MessagesAddressees = new HashSet<MessagesAddressee>();
    }

    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string? Head { get; set; }
    public string? Body { get; set; }
    public DateTime CreateDate { get; set; }
    public int? ReplyId { get; set; }
    
    public virtual User Author { get; set; } = null!;
    public virtual ICollection<MessagesAddressee> MessagesAddressees { get; set; }
}