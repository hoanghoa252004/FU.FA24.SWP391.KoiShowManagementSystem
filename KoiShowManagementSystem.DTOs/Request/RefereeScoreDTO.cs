using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class RefereeScoreDTO
    {
        //public int RefereeDetailId { get; set; }
        public List<ScoreDetailDTO> ScoreDetail { get; set; } = new List<ScoreDetailDTO>();
    }
}
