using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
