using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BMMT_NC.Models;

public partial class CsdlContext : DbContext
{
    public CsdlContext()
    {
    }

    public CsdlContext(DbContextOptions<CsdlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentLike> CommentLikes { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<Hashtag> Hashtags { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessagePhoto> MessagePhotos { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=SOPHY;Initial Catalog=CSDL;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId }).HasName("PK__bookmark__CA534F7919EEC356");

            entity.ToTable("bookmarks");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Post).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookmarks__post___656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookmarks__user___66603565");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__comments__E795768707D77CCA");

            entity.ToTable("comments");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CommentText)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("comment_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comments__post_i__693CA210");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comments__user_i__6A30C649");
        });

        modelBuilder.Entity<CommentLike>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CommentId }).HasName("PK__comment___D7C7606793835CA9");

            entity.ToTable("comment_likes");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentLikes)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comment_l__comme__6754599E");

            entity.HasOne(d => d.User).WithMany(p => p.CommentLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comment_l__user___68487DD7");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FolloweeId }).HasName("PK__follows__710D19E632B88689");

            entity.ToTable("follows");

            entity.Property(e => e.FollowerId).HasColumnName("follower_id");
            entity.Property(e => e.FolloweeId).HasColumnName("followee_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Followee).WithMany(p => p.FollowFollowees)
                .HasForeignKey(d => d.FolloweeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__follows__followe__6C190EBB");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__follows__followe__6B24EA82");
        });

        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.HasKey(e => e.HashtagId).HasName("PK__hashtags__F59C84ECBEB09B77");

            entity.ToTable("hashtags");

            entity.HasIndex(e => e.HashtagName, "UQ__hashtags__47D1FA87B6272505").IsUnique();

            entity.Property(e => e.HashtagId).HasColumnName("hashtag_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.HashtagName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("hashtag_name");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__login__C2C971DBC65465EA");

            entity.ToTable("login");

            entity.Property(e => e.LoginId).HasColumnName("login_id");
            entity.Property(e => e.Ip)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ip");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("login_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__login__user_id__6D0D32F4");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__messages__0BBF6EE6AECC327C");

            entity.ToTable("messages");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.MessageText)
                .IsUnicode(false)
                .HasColumnName("message_text");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__messages__receiv__6EF57B66");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__messages__sender__6FE99F9F");
        });

        modelBuilder.Entity<MessagePhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__message___3213E83F25C831C4");

            entity.ToTable("message_photos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .HasColumnName("photo_url");

            entity.HasOne(d => d.Message).WithMany(p => p.MessagePhotos)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__message_p__messa__6E01572D");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F997BA2CA");

            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__user___70DDC3D8");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__photos__CB48C83D480C94A7");

            entity.ToTable("photos");

            entity.HasIndex(e => e.PhotoUrl, "UQ__photos__1464808B5FF73123").IsUnique();

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("photo_url");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Size).HasColumnName("size");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__post__3ED78766C9F28A41");

            entity.ToTable("post");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Caption)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("caption");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VideoId).HasColumnName("video_id");

            entity.HasOne(d => d.Photo).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("FK__post__photo_id__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__post__user_id__72C60C4A");

            entity.HasOne(d => d.Video).WithMany(p => p.Posts)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK__post__video_id__73BA3083");

            entity.HasMany(d => d.Hashtags).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostTag",
                    r => r.HasOne<Hashtag>().WithMany()
                        .HasForeignKey("HashtagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__post_tags__hasht__76969D2E"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__post_tags__post___778AC167"),
                    j =>
                    {
                        j.HasKey("PostId", "HashtagId").HasName("PK__post_tag__E18E4F284E87E1B0");
                        j.ToTable("post_tags");
                        j.IndexerProperty<int>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<int>("HashtagId").HasColumnName("hashtag_id");
                    });
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId }).HasName("PK__post_lik__CA534F792052BB89");

            entity.ToTable("post_likes");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__post_like__post___74AE54BC");

            entity.HasOne(d => d.User).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__post_like__user___75A278F5");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F08E557A7");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC572A52CD253").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.ProfilePhotoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("https://picsum.photos/100")
                .HasColumnName("profile_photo_url");
            entity.Property(e => e.Token)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK__videos__E8F11E104F3B5032");

            entity.ToTable("videos");

            entity.HasIndex(e => e.VideoUrl, "UQ__videos__36C6F592542CA734").IsUnique();

            entity.Property(e => e.VideoId).HasColumnName("video_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Size).HasColumnName("size");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("video_url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
