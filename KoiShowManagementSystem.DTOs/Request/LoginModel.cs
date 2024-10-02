namespace KoiShowManagementSystem.DTOs.Request
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
