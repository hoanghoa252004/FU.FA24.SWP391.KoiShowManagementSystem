﻿using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<RefereeDetail> RefereeDetails { get; set; } = new List<RefereeDetail>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual ICollection<Koi> Kois { get; set; } = new List<Koi>();
}
