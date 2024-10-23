using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class ShowModel
    {
        public int ShowId { get; set; }
        public string? ShowTitle { get; set; }
        public string? ShowBanner { get; set; }
        public string? ShowDesc { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly? RegistrationStartDate { get; set; }
        public DateOnly? RegistrationCloseDate { get; set; }
        public string? ShowStatus { get; set; }
        public List<GroupModel>? ShowGroups { get; set; }
        public List<RefereeModel>? ShowReferee { get; set; }
        public decimal? EntranceFee { get; set; }
    }
}
