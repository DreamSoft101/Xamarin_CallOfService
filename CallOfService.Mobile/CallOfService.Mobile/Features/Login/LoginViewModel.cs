using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PropertyChanged;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Login
{
    [ImplementPropertyChanged]
    public class LoginViewModel : IViewAwareViewModel
    {
        private readonly ILoginService _loginService;

        public LoginViewModel(ILoginService loginService)
        {
            _loginService = loginService;
			Relogin = false;
        }


		public bool Relogin {get;set;}
        public string Username { get; set; }
        public string Password { get; set; }

        public bool ShowErrorMessage { get; set; }

        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var loginResult = await _loginService.Login(Username, Password);
                    if (loginResult.IsSuccessful)
						{
							if(Relogin)
								await NavigationService.Dismiss();
							else
								await NavigationService.NavigateToWelcomeScreen();
						}
                    else
                        ShowErrorMessage = true;
                });
            }
        }

        public void Dispose()
        {
        }

        public void OnAppearing()
        {
        }

        public void OnDisappearing()
        {
        }
    }
}
