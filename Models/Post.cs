using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int? PhotoId { get; set; }

    public int? VideoId { get; set; }

    public int UserId { get; set; }

    public string? Caption { get; set; }

    public string? Location { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Photo? Photo { get; set; }

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual User User { get; set; } = null!;

    public virtual Video? Video { get; set; }

    public virtual ICollection<Hashtag> Hashtags { get; set; } = new List<Hashtag>();
}
