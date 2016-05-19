using System;
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

			this.Subscribe<NewDateSelected>(mn =>
			{
				IsPresented = false;
			});

			this.Subscribe<Logout>(m => IsPresented = false);

			NavigationPage.SetHasNavigationBar(this, false);
			var mainMenuPage = new MainMenuPage()
			{
				Title = "Call Of Service",
				Icon = "MenuIcon.png",
				BindingContext = DependencyResolver.Resolve<MainMenuViewModel>()
			};

			MasterBehavior = MasterBehavior.Default;
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