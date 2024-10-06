using KoiShowManagementSystem.DTOs.Request;

namespace KoiShowManagementSystem.API.Helper
{
    public interface IEmailService
    {
        Task SendEmail(EmailModel emailRequest);
    }
}