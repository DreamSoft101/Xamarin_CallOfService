using CallOfService.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Welcome
{
    public partial class WelcomePage : BasePage
    {
        WelcomeViewModel viewModel;

        public WelcomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            viewModel = (WelcomeViewModel)BindingContext;
            var username = await viewModel.GetUserName();

            try
            {
                await Message1.FadeTo(0);
                await Message2.FadeTo(0);

                Message1.Text = "Hi";
                Message2.Text = username;

                await Message1.FadeTo(100);
                await Message2.FadeTo(100);
            }
            catch
            {
                // Ignore exception.
            }

            if (viewModel.NavigateToDashboard.CanExecute(null))
                viewModel.NavigateToDashboard.Execute(null);
        }
    }
}
