using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Database.Repos.Abstracts
{
	public interface IUserRepo
	{
		Task<int> SaveUserProfile (UserProfile userProfile);

		Task<UserProfile> GetCurrentUserProfile ();

		Task DeleteUserProfile ();
	}
}
