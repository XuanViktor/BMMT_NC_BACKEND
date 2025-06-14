using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class CommentLike
{
    public int UserId { get; set; }

    public int CommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
