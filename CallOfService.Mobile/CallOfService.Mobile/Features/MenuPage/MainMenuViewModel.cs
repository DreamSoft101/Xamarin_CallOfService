using System;
using System.Windows.Input;
using Xamarin.Forms;
using CallOfService.Mobile.Services.Abstracts;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.SystemServices;
using PubSub;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.UI;

namespace CallOfService.Mobile
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserDialogs _userDialogs;

        public MainMenuViewModel(ILoginService loginService, IUserDialogs userDialogs)
        {
            _loginService = loginService;
            _userDialogs = userDialogs;
        }

        public ICommand LogoutCommand
        {
            get
            {
                return new Command(async () =>
                {
                    _userDialogs.ShowLoading("Loging out ...");
                    var logout = await _loginService.Logout();
                    if (logout)
                    {
                        _userDialogs.HideLoading();
                        this.Publish(new Logout());
                    }
                    else
                    {
                        _userDialogs.ShowError("Error while logging out!");
                    }
                });
            }
        }

        public ICommand ShowJobsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var isJobDetailsLastPage = NavigationService.IsJobDetailsPresent();
                    if (isJobDetailsLastPage)
                        await NavigationService.NavigateToRootAsync();
                    this.Publish(new NewDateSelected(DateTime.Now));
                });
            }
        }

        public ICommand ShowSettingsCommand
        {
            get
            {
                return new Command(() =>
                {
                    _userDialogs.ShowLoading("Loading ...");
                    this.Publish(new NavigateToSettings());
                    _userDialogs.HideLoading();
                });
            }
        }
    }
}