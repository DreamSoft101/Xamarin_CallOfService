using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Database.Repos.Abstracts;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly IDbSet<UserProfile> _userProfileDbSet;

        public UserRepo(IDbSet<UserProfile> userProfileDbSet)
        {
            _userProfileDbSet = userProfileDbSet;
        }

        public Task<int> SaveUserProfile(UserProfile userProfile)
        {
            return _userProfileDbSet.Add(userProfile);
        }
    }
}