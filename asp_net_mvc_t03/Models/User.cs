namespace asp_net_mvc_t03.Models;

public partial class User
{
    public User()
    {
        Messages = new HashSet<Message>();
        MessagesAddressees = new HashSet<MessagesAddressee>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public string Status { get; set; } = null!;
    public string Password { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<MessagesAddressee> MessagesAddressees { get; set; }
}