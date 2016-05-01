using System;
using Xamarin.Forms;
using CallOfService.Technician.Mobile.Features.Dashboard;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Core.DI;

namespace CallOfService.Technician.Mobile
{
	public class MasterDetailMainPage : MasterDetailPage
	{
		public MasterDetailMainPage ()
		{
			NavigationPage.SetHasNavigationBar (this, false);
			var mainMenuPage = new MainMenuPage (){
				Title = "Call Of Service",
				Icon = "MenuIcon.png",
				BindingContext = DependencyResolver.Resolve<MainMenuViewModel>()
			};

			MasterBehavior = MasterBehavior.Split;
			Master = mainMenuPage;
			Detail = new NavigationPage(new DashboardPage ()){
				BarBackgroundColor = Color.FromHex("#44b6ae"),
				BarTextColor = Color.White
			};
			NavigationService.Navigation = Detail.Navigation;
		}
	}
}