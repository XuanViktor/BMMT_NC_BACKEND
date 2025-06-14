using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class Hashtag
{
    public int HashtagId { get; set; }

    public string? HashtagName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
