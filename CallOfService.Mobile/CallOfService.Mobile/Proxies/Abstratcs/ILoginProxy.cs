using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Proxies.Abstratcs
{
	public interface ILoginProxy
	{
		Task<UserToken> Login (string userName, string password);

		Task<bool> Logout ();
	}
}
