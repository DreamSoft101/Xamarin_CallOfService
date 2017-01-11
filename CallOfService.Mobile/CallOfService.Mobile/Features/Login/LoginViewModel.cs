using System;
using System.Windows.Input;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.Networking;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Login
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserDialogs _userDialogs;

        public LoginViewModel(ILoginService loginService, IUserDialogs userDialogs)
        {
            _loginService = loginService;
            _userDialogs = userDialogs;
            Relogin = false;
        }

        private bool _relogin;
        public bool Relogin
        {
            get { return _relogin; }
            set { SetPropertyValue(ref _relogin, value); }
        }

        private string _userName;
        public string Username
        {
            get { return _userName; }
            set { SetPropertyValue(ref _userName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetPropertyValue(ref _password, value); }
        }

        private bool _showErrorMessage;
        public bool ShowErrorMessage
        {
            get { return _showErrorMessage; }
            set { SetPropertyValue(ref _showErrorMessage, value); }
        }

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
                            await NavigationService.DismissAsync();
                        else
                        {
							//await NavigationService.NavigateToRootAsync();
							App.ShowMainPage();
                        }
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
    }
}
