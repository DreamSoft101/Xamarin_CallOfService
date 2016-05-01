using System;
using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;
using CallOfService.Technician.Mobile.Services.Abstracts;
using Acr.UserDialogs;
using CallOfService.Technician.Mobile.Core.SystemServices;
using PubSub;

namespace CallOfService.Technician.Mobile
{
	[ImplementPropertyChanged]
	public class MainMenuViewModel
	{
		private ILoginService _loginService;
		private IUserDialogs _userDialogs;

		public MainMenuViewModel (ILoginService loginService, IUserDialogs userDialogs)
		{
			_loginService = loginService;
			_userDialogs = userDialogs;
		}

		public ICommand LogoutCommand {
			get { 
				return new Command (async () => {
					_userDialogs.ShowLoading("Loging out ...");
					var logout = await _loginService.Logout ();
					if(logout){
						_userDialogs.HideLoading();
						this.Publish(new Logout());	
					}else{
						_userDialogs.ShowError("Error");
					}
				});
			}
		}
	}
}

