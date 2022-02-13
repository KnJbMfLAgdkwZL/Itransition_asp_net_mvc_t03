using Microsoft.EntityFrameworkCore;

namespace asp_net_mvc_t03.Models;

public partial class MasterContext : DbContext
{
    public MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Message> Messages { get; set; } = null!;
    public virtual DbSet<MessagesAddressee> MessagesAddressees { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        const string schema = "task3";
        
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages", schema);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.Body)
                .IsUnicode(false)
                .HasColumnName("body");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.Head)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("head");
            entity.Property(e => e.ReplyId).HasColumnName("replyId");
            entity.HasOne(d => d.Author)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_users_id_fk");
        });
        
        modelBuilder.Entity<MessagesAddressee>(entity =>
        {
            entity.ToTable("messagesAddressee", schema);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.New).HasColumnName("new");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.HasOne(d => d.Message)
                .WithMany(p => p.MessagesAddressees)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messagesAddressee_messages_id_fk");
            entity.HasOne(d => d.User)
                .WithMany(p => p.MessagesAddressees)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messagesAddressee_users_id_fk");
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", schema);
            entity.HasIndex(e => e.Email, "users_mail_uindex")
                .IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.LastLoginDate).HasColumnName("lastLoginDate");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RegistrationDate).HasColumnName("registrationDate");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}