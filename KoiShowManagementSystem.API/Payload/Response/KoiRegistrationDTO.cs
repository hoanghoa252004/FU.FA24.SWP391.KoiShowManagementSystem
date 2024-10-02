namespace KoiShowManagementSystem.API.Payload.Response
{
    public class KoiRegistrationDTO
    {
        public int Id { get; set; }
        public string? KoiName { get; set; }
        public string? Description { get; set; }
        public decimal? Size { get; set; }
        public string? Variety { get; set; }
        public string? Show { get; set; }
        public string? Group { get; set; }
        public string? Status { get; set; }
        public decimal? TotalScore { get; set; }
        public string? Image { get; set; }
        public string? Video { get; set; }

    }
}
