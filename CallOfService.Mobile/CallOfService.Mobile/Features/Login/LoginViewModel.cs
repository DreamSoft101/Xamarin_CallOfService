using System;
using System.Windows.Input;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.Networking;
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

        public ICommand ChangeServerUrlCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var currentUrlString = Helpers.Settings.ServerUrl;
                    var currentUrl = new Uri(currentUrlString);
                    var currentDomain = currentUrl.Host;

                    var result = await _userDialogs.PromptAsync(new PromptConfig
                    {
                        Title = "Server Host",
                        Message = "Only for custom plans",
                        Text = currentDomain,
                        //Placeholder = "Placeholder",
                        InputType = InputType.Default,
                        OkText = "Save",
                        CancelText = "Cancel/Reset"
                    });
                    
                    if (!result.Ok)
                        Helpers.Settings.ServerUrl = UrlConstants.BaseUrlDefault;
                    else if (result.Value.ToLower() != Helpers.Settings.ServerUrl)
                    {
                        var domain = result.Value.ToLower();
                        var baseUrl = $"https://{domain}/api/";
                        try
                        {
                            new Uri(baseUrl);
                        }
                        catch (Exception)
                        {
                            _userDialogs.ShowError("Provided host is not valid.");
                        }
                        var isValid = await BaseProxy.IsValidBaseUrl(baseUrl);
                        if(isValid)
                            Helpers.Settings.ServerUrl = baseUrl;
                        else
                        {
                            _userDialogs.ShowError("Provided host is not valid.");
                        }
                    }
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
