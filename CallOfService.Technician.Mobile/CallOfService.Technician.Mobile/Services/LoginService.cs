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

        public async Task<UserLoginResult> Login(string userName, string password)
        {
            UserToken userToken = await _loginProxy.Login(userName, password);
            if (userToken != null && userToken.UserProfile != null && userToken.Token != null)
            {
                return new UserLoginResult(true, userToken.UserProfile);
            }
            return new UserLoginResult(false, null);
        }
    }

    public class UserLoginResult
    {
        public UserLoginResult(bool isSuccessful, UserProfile userProfile)
        {
            IsSuccessful = isSuccessful;
            UserProfile = userProfile;
        }

        public UserProfile UserProfile { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
