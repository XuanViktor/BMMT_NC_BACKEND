using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class MessagePhoto
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public virtual Message Message { get; set; } = null!;
}
