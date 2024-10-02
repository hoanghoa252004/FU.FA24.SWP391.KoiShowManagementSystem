using System.Collections.Generic;

namespace KoiShowManagementSystem.API.Payload.Response
{
    public class KoiShowDTO
    {
        public int ShowId { get; set; }
        public string? ShowTitle { get; set; }
        public DateTime ShowDate { get; set; }
        public string? Description { get; set; }
        public List<string> GroupName { get; set; }
        public List<string> RefereeName { get; set; }
        public int Status { get; set; }
        public string? Banner { get; set; }

    }
}
