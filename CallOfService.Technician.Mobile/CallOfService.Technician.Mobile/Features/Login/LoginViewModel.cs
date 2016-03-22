using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PropertyChanged;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.Login
{
    [ImplementPropertyChanged]
    public class LoginViewModel : IViewAwareViewModel
    {
        private readonly ILoginService _loginService;

        public LoginViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var userProfile = await _loginService.Login(Username, Password);
                    UserDialogs.Instance.Alert(userProfile.FirstName);
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
