using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Features.Login;
using Xamarin.Forms;
using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Core.Security;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Database;
using CallOfService.Technician.Mobile.Features.Dashboard;
using CallOfService.Technician.Mobile.Services.Abstracts;

namespace CallOfService.Technician.Mobile
{
    public partial class App : Application
    {
        public App()
        {
        }

        protected async override void OnStart()
        {
            MainPage = new Page();
            await InitDB();
            if (ShouldLogin())
            {
                var loginPage = new LoginPage { BindingContext = DependencyResolver.Resolve<LoginViewModel>() };
                MainPage = new NavigationPage(loginPage);
            }
            else
            {
                MainPage = new NavigationPage(new DashboardPage());
            }
            
            NavigationService.Navigation = MainPage.Navigation;
        }

        private bool ShouldLogin()
        {
            var userService = DependencyResolver.Resolve<IUserService>();
            var userCredentials = userService.GetUserCredentials();
            return userCredentials == null;
        }

        private async Task InitDB()
        {
            await DependencyResolver.Resolve<DbInitializer>().InitDb();
        }
    }
}
