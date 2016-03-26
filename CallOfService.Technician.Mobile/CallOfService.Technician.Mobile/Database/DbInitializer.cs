using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database
{
    public class DbInitializer
    {
        private readonly IDbSet<UserProfile> _userProfileDbSet;

        public DbInitializer(IDbSet<UserProfile> userProfileDbSet)
        {
            _userProfileDbSet = userProfileDbSet;
        }

        public async Task InitDb()
        {
            await _userProfileDbSet.CreateTable();
        }   
    }
}
