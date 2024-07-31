using System;
using System.Collections.Generic;

namespace DSTT_Backend.Database;

public partial class Message
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
