using System;
using System.Collections.Generic;

namespace KoiShowManagementSystem.Entities;

public partial class Score
{
    public int Id { get; set; }

    public int? KoiId { get; set; }

    public int? RefereeDetailId { get; set; }

    public decimal? Score1 { get; set; }

    public int? CriteriaId { get; set; }

    public virtual Criterion? Criteria { get; set; }

    public virtual KoiRegistration? Koi { get; set; }

    public virtual RefereeDetail? RefereeDetail { get; set; }
}
