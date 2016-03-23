using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Services.Abstracts
{
    public interface ILoginService
    {
        Task<UserLoginResult> Login(string userName, string password);
    }
}
