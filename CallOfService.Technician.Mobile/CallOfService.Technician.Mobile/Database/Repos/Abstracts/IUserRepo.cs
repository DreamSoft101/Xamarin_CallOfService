using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database.Repos.Abstracts
{
    public interface IUserRepo
    {
        Task<int> SaveUserProfile(UserProfile userProfile);
    }
}
