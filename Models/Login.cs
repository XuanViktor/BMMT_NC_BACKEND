using System;
using System.Collections.Generic;

namespace BMMT_NC.Models;

public partial class Login
{
    public int LoginId { get; set; }

    public int UserId { get; set; }

    public string Ip { get; set; } = null!;

    public DateTime? LoginTime { get; set; }

    public virtual User User { get; set; } = null!;
}
