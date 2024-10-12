using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Koi
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Size { get; set; }

    public string? Image { get; set; }

    public int? UserId { get; set; }

    public int VarietyId { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual User User { get; set; } = null!;

    public virtual Variety Variety { get; set; } = null!;
}
