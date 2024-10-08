using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Show
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ScoreStartDate { get; set; }

    public DateOnly? RegisterStartDate { get; set; }

    public DateOnly? RegisterEndDate { get; set; }

    public string? Banner { get; set; }

    public string? Status { get; set; }

    public DateOnly? ScoreEndDate { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<RefereeDetail> RefereeDetails { get; set; } = new List<RefereeDetail>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
