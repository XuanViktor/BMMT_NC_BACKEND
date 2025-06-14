using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string CommentText { get; set; } = null!;

    public int PostId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
