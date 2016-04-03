using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core.Security;
using CallOfService.Technician.Mobile.Database.Repos.Abstracts;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Services.Abstracts;

namespace CallOfService.Technician.Mobile.Services
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
            var credential = _credentialManager.Retrive(null);
            if (string.IsNullOrWhiteSpace(credential?.Email))
                return null;

            return _credentialManager.Retrive(credential.Email);
        }
    }
}