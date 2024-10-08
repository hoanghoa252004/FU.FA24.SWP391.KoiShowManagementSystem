using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Media
{
    public int Id { get; set; }

    public string Image1 { get; set; } = null!;

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public string Video { get; set; } = null!;

    public int RegistrationId { get; set; }

    public virtual Registration Registration { get; set; } = null!;
}
