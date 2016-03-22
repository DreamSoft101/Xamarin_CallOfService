using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Proxies.Abstratcs
{
    public interface ILoginProxy
    {
        Task<UserToken> Login(string userName, string password);
    }
}
