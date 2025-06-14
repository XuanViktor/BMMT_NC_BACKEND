using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class Photo
{
    public int PhotoId { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public int PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public double? Size { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
