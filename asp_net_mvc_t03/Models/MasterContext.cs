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
    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages", "task3");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AuthorId).HasColumnName("authorId");

            entity.Property(e => e.Body)
                .HasMaxLength(2550)
                .IsUnicode(false)
                .HasColumnName("body");

            entity.Property(e => e.CreateDate).HasColumnName("createDate");

            entity.Property(e => e.Head)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("head");

            entity.Property(e => e.New).HasColumnName("new");

            entity.Property(e => e.ReplyId).HasColumnName("replyId");

            entity.Property(e => e.ToUserId).HasColumnName("toUserId");

            entity.HasOne(d => d.Author)
                .WithMany(p => p.MessageAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_users_id_fk");

            entity.HasOne(d => d.ToUser)
                .WithMany(p => p.MessageToUsers)
                .HasForeignKey(d => d.ToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_users_id_fk_2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "task3");

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