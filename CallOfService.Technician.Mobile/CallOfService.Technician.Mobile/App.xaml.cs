using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Features.Login;
using Xamarin.Forms;
using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Core.SystemServices;

namespace CallOfService.Technician.Mobile
{
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStart()
        {
            var loginPage = new LoginPage {BindingContext = DependencyResolver.Resolve<LoginViewModel>()};
            MainPage = new NavigationPage(loginPage);
            NavigationService.Navigation = MainPage.Navigation;
        }
    }
}
