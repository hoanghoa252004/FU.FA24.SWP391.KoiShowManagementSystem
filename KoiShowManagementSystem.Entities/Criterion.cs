using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Criterion
{
    public int Id { get; set; }

    public int? GroupId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Percentage { get; set; }

    public bool Status { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
