using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using CallOfService.Technician.Mobile.Services.Abstracts;

namespace CallOfService.Technician.Mobile.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginProxy _loginProxy;

        public LoginService(ILoginProxy loginProxy)
        {
            _loginProxy = loginProxy;
        }

        public async Task<UserProfile> Login(string userName, string password)
        {
            UserToken userToken = await _loginProxy.Login(userName, password);
            return userToken.UserProfile;
        }
    }
}
