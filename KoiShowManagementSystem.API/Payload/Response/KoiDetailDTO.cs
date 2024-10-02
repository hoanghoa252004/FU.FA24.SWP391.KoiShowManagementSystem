namespace KoiShowManagementSystem.API.Payload.Response
{
    public class KoiDetailDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Size { get; set; }
        public string? GroupName { get; set; }
        public decimal? TotalScore { get; set; }
        public bool? IsPaid { get; set; }
        public string? Status { get; set; }
        public int? Rank { get; set; }
        public bool? IsBestVote { get; set; }
        public string? VarietyName { get; set; }
        public string? UserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? image { get; set; }
        public string? ShowTitle { get; set; }
        public int VoteCount { get; set; }
    }
}
