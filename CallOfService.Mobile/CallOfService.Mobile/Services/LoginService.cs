using System.Threading.Tasks;
using CallOfService.Mobile.Core.Security;
using CallOfService.Mobile.Database.Repos.Abstracts;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginProxy _loginProxy;
        private readonly IUserRepo _userRepo;
        private readonly ICredentialManager _credentialManager;

        public LoginService(ILoginProxy loginProxy, IUserRepo userRepo, ICredentialManager credentialManager)
        {
            _loginProxy = loginProxy;
            _userRepo = userRepo;
            _credentialManager = credentialManager;
        }

        public async Task<UserLoginResult> Login(string userName, string password)
        {
            UserToken userToken = await _loginProxy.Login(userName, password);
            if (userToken?.UserProfile != null && userToken.Token != null)
            {
                await _userRepo.SaveUserProfile(userToken.UserProfile);
                _credentialManager.Save(new Credential(userName, password, userToken.Token.TokenId));
                return new UserLoginResult(true, userToken.UserProfile);
            }
            return new UserLoginResult(false, null);
        }

        public async Task<bool> Logout()
        {
            var logoutSuccessful = await _loginProxy.Logout();
            if (logoutSuccessful)
            {
                await _userRepo.DeleteUserProfile();
                _credentialManager.Delete();
                return true;
            }
            return false;
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
