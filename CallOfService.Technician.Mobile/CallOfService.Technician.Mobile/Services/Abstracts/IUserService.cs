using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core.Security;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Services.Abstracts
{
    public interface IUserService
    {
        Task<UserProfile> GetCurrentUser();
        Credential GetUserCredentials();
    }
}
