using System.Threading.Tasks;
using CallOfService.Mobile.Core.Security;
using CallOfService.Mobile.Database.Repos.Abstracts;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly ICredentialManager _credentialManager;

        public UserService(IUserRepo userRepo, ICredentialManager credentialManager)
        {
            _userRepo = userRepo;
            _credentialManager = credentialManager;
        }

        public Task<UserProfile> GetCurrentUser()
        {
            return _userRepo.GetCurrentUserProfile();
        }

        public Credential GetUserCredentials()
        {
            var credential = _credentialManager.Retrive();
            if (string.IsNullOrWhiteSpace(credential?.Email))
                return null;

            return _credentialManager.Retrive();
        }
    }
}