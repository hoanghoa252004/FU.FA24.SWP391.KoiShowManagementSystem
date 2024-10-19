using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class ScoreDetailDTO
    {
        public int RegistraionId { get; set; }
        public List<CriterionScoreDTO> Scores { get; set; } = new List<CriterionScoreDTO>();
    }
}
