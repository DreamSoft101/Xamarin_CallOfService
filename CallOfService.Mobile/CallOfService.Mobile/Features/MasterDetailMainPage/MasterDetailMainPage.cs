using Xamarin.Forms;
using CallOfService.Mobile.Features.Dashboard;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Core.DI;
using PubSub;
using CallOfService.Mobile.Messages;

namespace CallOfService.Mobile
{
	public class MasterDetailMainPage : MasterDetailPage
	{
		public MasterDetailMainPage()
		{
		    this.Subscribe<NewDateSelected>(m => IsPresented = false);
			this.Subscribe<Logout>(m => IsPresented = false);

			NavigationPage.SetHasNavigationBar(this, false);

			var mainMenuPage = new MainMenuPage
			{
				Title = "Call of Service",
				Icon = "MenuIcon.png",
				BindingContext = DependencyResolver.Resolve<MainMenuViewModel>()
			};

			MasterBehavior = MasterBehavior.Popover;
			IsPresented = false;
			Master = mainMenuPage;
			Detail = new NavigationPage(new DashboardPage())
			{
				BarBackgroundColor = Color.FromHex("#44b6ae"),
				BarTextColor = Color.White
			};
			NavigationService.Navigation = Detail.Navigation;
		}
	}
}