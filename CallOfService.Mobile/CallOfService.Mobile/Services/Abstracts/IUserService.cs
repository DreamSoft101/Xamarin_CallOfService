using System.Threading.Tasks;
using CallOfService.Mobile.Core.Security;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Services.Abstracts
{
    public interface IUserService
    {
        Task<UserProfile> GetCurrentUser();
        Credential GetUserCredentials();
    }
}
