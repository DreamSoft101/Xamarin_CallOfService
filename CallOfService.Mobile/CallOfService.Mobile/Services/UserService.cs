using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.Security;
using CallOfService.Mobile.Database.Repos.Abstracts;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Services
{
    public class UserService : IUserService
    {
        private static readonly AsyncLock Mutex = new AsyncLock();

        private readonly IUserRepo _userRepo;
        private readonly ICredentialManager _credentialManager;
        private UserProfile _currentUserProfile;
        private DateTime? _lastUpdated;

        public UserService(IUserRepo userRepo, ICredentialManager credentialManager)
        {
            _userRepo = userRepo;
            _credentialManager = credentialManager;
        }

        public async Task<UserProfile> GetCurrentUser()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                if (NeedsRefresh())
                {
                    _currentUserProfile = await _userRepo.GetCurrentUserProfile();
                    _lastUpdated = DateTime.UtcNow;
                }

                return _currentUserProfile;
            }
        }

        public Credential GetUserCredentials()
        {
            var credential = _credentialManager.Retrive();
            if (string.IsNullOrWhiteSpace(credential?.Email))
                return null;

            return _credentialManager.Retrive();
        }

        private bool NeedsRefresh()
        {
            return _currentUserProfile == null || _lastUpdated == null || (DateTime.UtcNow - _lastUpdated.Value).TotalMinutes > 5;
        }
    }
}