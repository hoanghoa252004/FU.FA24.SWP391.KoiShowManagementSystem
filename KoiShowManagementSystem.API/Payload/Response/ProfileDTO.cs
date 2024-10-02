namespace KoiShowManagementSystem.API.Payload.Response
{
    public class ProfileDTO
    {
        public int Id { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public string? Name { get; set; }

        public bool? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

    }
}
