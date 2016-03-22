using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.UI
{
    public class BasePage : ContentPage
    {

        public BasePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            var viewAwareViewModel = BindingContext as IViewAwareViewModel;
            viewAwareViewModel?.OnAppearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            var viewAwareViewModel = BindingContext as IViewAwareViewModel;
            viewAwareViewModel?.OnDisappearing();
            base.OnDisappearing();
        }
    }
}