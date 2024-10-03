using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IKoiRegistrationService
    {
        Task<IEnumerable<object>> GetMyKoiRegistration(string status);
    }
}
