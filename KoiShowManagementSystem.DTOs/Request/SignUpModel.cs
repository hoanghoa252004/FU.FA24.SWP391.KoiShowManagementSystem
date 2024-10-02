namespace KoiShowManagementSystem.DTOs.Request
{
    public class SignUpModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
    }
}
