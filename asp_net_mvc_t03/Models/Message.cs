﻿namespace asp_net_mvc_t03.Models;

public partial class Message
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int ToUserId { get; set; }
    public string Head { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int? ReplyId { get; set; }
    public bool New { get; set; }
    public string Uid { get; set; } = null!;

    public virtual User Author { get; set; } = null!;
    public virtual User ToUser { get; set; } = null!;
}