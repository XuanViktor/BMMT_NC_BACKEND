using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class MessagePhoto
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int PhotoUrl { get; set; }

    public virtual Message Message { get; set; } = null!;
}
