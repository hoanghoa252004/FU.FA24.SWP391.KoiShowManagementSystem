using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services.Helper
{
    public interface IEmailService
    {
        Task SendEmail(EmailModel emailRequest);
    }
}
