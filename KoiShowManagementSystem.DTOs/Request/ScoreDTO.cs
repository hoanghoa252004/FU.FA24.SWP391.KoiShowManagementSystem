using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class ScoreDTO
    {
        public int CriterionId { get; set; }
        public int KoiId { get; set; }
        public int RefereeDetailId { get; set; }
        public decimal ScoreValue { get; set; }
    }
}
