namespace asp_net_mvc_t03.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public string Status { get; set; } = null!;
    public string Password { get; set; } = null!;
}