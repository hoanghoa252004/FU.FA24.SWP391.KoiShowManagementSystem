using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Group
{
    public int Id { get; set; }

    public int? ShowId { get; set; }

    public string? Name { get; set; }

    public decimal? SizeMin { get; set; }

    public decimal? SizeMax { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual Show? Show { get; set; }

    public virtual ICollection<Variety> Varieties { get; set; } = new List<Variety>();
}
