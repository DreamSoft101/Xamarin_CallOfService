using System;
using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;
using CallOfService.Mobile.Services.Abstracts;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.SystemServices;
using PubSub;
using CallOfService.Mobile.Messages;

namespace CallOfService.Mobile
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

		public ICommand ShowJobsCommand{
			get{ 
				return new Command (async ()=> {
					var isJobDetailsLastPage = NavigationService.IsJobDetailsPresent();
					if(isJobDetailsLastPage)
						await NavigationService.NavigateBack();
					this.Publish(new NewDateSelected(DateTime.Now));
				});
			}
		}
	}
}