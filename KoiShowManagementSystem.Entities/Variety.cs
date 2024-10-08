using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Variety
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Origin { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Koi> Kois { get; set; } = new List<Koi>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
