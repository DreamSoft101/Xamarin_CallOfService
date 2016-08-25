using System.Threading.Tasks;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Database;
using CallOfService.Mobile.Features.Login;
using CallOfService.Mobile.Services.Abstracts;
using PubSub;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CallOfService.Mobile
{
    public partial class App : Application
    {
        public static string AppName => "CallOfService.Mobile";

        public App()
        {
            this.Subscribe<Logout>(async m =>
            {
                NavigationService.MainNavigation = NavigationService.Navigation;
                await NavigationService.NavigateToLoginPageAsync();
            });

            this.Subscribe<NavigateToSettings>(async m =>
            {
                NavigationService.MainNavigation = NavigationService.Navigation;
                await NavigationService.NavigateToSettingsPageAsync();
            });
        }

        protected override async void OnStart()
        {
            var analyticsService = DependencyResolver.Resolve<IAnalyticsService>();
            analyticsService.Initialize();

            MainPage = new Page();
            await InitDB();
            if (await ShouldLogin())
            {
                var loginPage = new LoginPage { BindingContext = DependencyResolver.Resolve<LoginViewModel>() };
                MainPage = new NavigationPage(loginPage);
                NavigationService.MainNavigation = MainPage.Navigation;
            }
            else
            {
                MainPage = new MasterDetailMainPage();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async Task<bool> ShouldLogin()
        {
            var userService = DependencyResolver.Resolve<IUserService>();
            var currentUser = await userService.GetCurrentUser();
            return currentUser == null;
        }

        private async Task InitDB()
        {
            await DependencyResolver.Resolve<DbInitializer>().InitDb();
        }
    }
}
