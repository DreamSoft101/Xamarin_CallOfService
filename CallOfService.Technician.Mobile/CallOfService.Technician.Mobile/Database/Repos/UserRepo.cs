﻿using System.Collections.Generic;
using System.Linq;
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

		public async Task<int> SaveUserProfile(UserProfile userProfile)
        {
			await _userProfileDbSet.DeleteAll ();
			return await  _userProfileDbSet.Add(userProfile);
        }

        public async Task<UserProfile> GetCurrentUserProfile()
        {
            var userProfiles = await _userProfileDbSet.GetAllAsync();
            return userProfiles.First();
        }
    }
}