using System.Threading.Tasks;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Features.Login;
using Xamarin.Forms;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Database;
using CallOfService.Mobile.Features.Dashboard;
using CallOfService.Mobile.Services.Abstracts;
using PubSub;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CallOfService.Mobile
{
	public partial class App : Application
	{
		public App()
		{
			this.Subscribe<Logout>(async m =>
			{
				NavigationService.MainNavigation = NavigationService.Navigation;
				await NavigationService.NaviGateToLoginPage();
				//var loginPage = new LoginPage { BindingContext = DependencyResolver.Resolve<LoginViewModel> () };
				//MainPage = new NavigationPage (loginPage);
				//NavigationService.Navigation = MainPage.Navigation;
			});
		}

		public static string AppName => "CallOfService.Mobile";

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