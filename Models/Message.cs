using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public bool? IsRead { get; set; }

    public virtual ICollection<MessagePhoto> MessagePhotos { get; set; } = new List<MessagePhoto>();

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
