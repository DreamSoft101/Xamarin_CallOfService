using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Services.Abstracts
{
	public interface ILoginService
	{
		Task<UserLoginResult> Login (string userName, string password);

		Task<bool> Logout ();
	}
}
