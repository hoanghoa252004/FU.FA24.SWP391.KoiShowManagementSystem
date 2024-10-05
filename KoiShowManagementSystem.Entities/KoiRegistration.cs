using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class KoiRegistration
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Size { get; set; }

    public int GroupId { get; set; }

    public DateOnly? CreateDate { get; set; }

    public decimal? TotalScore { get; set; }

    public bool? IsPaid { get; set; }

    public string? Status { get; set; }

    public int? UserId { get; set; }

    public int? VarietyId { get; set; }

    public int? Rank { get; set; }

    public bool? IsBestVote { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Illustration? Illustration { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual User? User { get; set; }

    public virtual Variety? Variety { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
