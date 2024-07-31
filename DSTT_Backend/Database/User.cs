using System;
using System.Collections.Generic;

namespace DSTT_Backend.Database;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<Follow> FollowFolloweds { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
