﻿using Newtonsoft.Json;

namespace asp_net_mvc_t03.Models;

public partial class User
{
    public User()
    {
        MessageAuthors = new HashSet<Message>();
        MessageToUsers = new HashSet<Message>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public string Status { get; set; } = null!;
    [JsonIgnore] public string Password { get; set; } = null!;

    [JsonIgnore] public virtual ICollection<Message> MessageAuthors { get; set; }
    [JsonIgnore] public virtual ICollection<Message> MessageToUsers { get; set; }
}