using System;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Features.Dashboard;
using CallOfService.Technician.Mobile.Features.JobDetails;
using CallOfService.Technician.Mobile.Features.Welcome;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Core.SystemServices
{
    public class NavigationService
    {
        public static INavigation Navigation { get; set; }

        public static Task NavigateToWelcomeScreen()
        {
            return Navigation.PushAsync(CreateAndBind<WelcomePage>(DependencyResolver.Resolve<WelcomeViewModel>()));
        }

        public static Page CreateAndBind<T>(object obj) where T : Page, new()
        {
            T page;
            var weakReference = new WeakReference<T>(new T { BindingContext = obj });
            weakReference.TryGetTarget(out page);
            return page;
        }

        public static Task NavigateToDashboardScreen()
        {
            return Navigation.PushAsync(CreateAndBind<DashboardPage>(new object()));
        }

		public static Task NavigateToJobDetails(){
			return Navigation.PushAsync (CreateAndBind<JobDetailsPage> (DependencyResolver.Resolve<JobDetailsViewModel> ()));
		}

		public static async  Task NavigateBack(){
			var page = await Navigation.PopAsync ();
			if (page.BindingContext is IDisposable) {
				(page.BindingContext as IDisposable).Dispose ();
			}
		}
    }
}
