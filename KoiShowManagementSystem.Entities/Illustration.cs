using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Illustration
{
    public int Id { get; set; }

    public int? KoiId { get; set; }

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public virtual KoiRegistration? Koi { get; set; }
}
