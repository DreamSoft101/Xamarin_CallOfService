using System.Windows.Input;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.SystemServices;
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
        private readonly IUserDialogs _userDialogs;

        public LoginViewModel(ILoginService loginService, IUserDialogs userDialogs)
        {
            _loginService = loginService;
            _userDialogs = userDialogs;
            Relogin = false;
        }

        public bool Relogin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool ShowErrorMessage { get; set; }

        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    _userDialogs.ShowLoading("Logging in...");

                    var loginResult = await _loginService.Login(Username, Password);
                    _userDialogs.HideLoading();
                    if (loginResult.IsSuccessful)
                    {
                        if (Relogin)
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
