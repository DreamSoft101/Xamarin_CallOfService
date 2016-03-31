using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.Welcome
{
    public partial class WelcomePage : BasePage
    {
		WelcomeViewModel viewModel;

        public WelcomePage()
        {
            InitializeComponent();
			viewModel = BindingContext  as WelcomeViewModel;
        }

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();

			var username  = await viewModel.GetUserName ();

			Message1.FadeTo (0, 250);
			await Message2.FadeTo (0, 250);

			Message1.Text = "HI";
			Message2.Text = username;

			Message1.FadeTo (100, 250);
			await Message2.FadeTo (100, 250);

			await Task.Delay (3000);

			Message1.FadeTo (0, 250);
			await Message2.FadeTo (0, 250);

			Message1.Text = "Welcome to";
			Message2.Text = "Call Of Service";

			Message1.FadeTo (100, 250);
			await Message2.FadeTo (100, 250);

			await Task.Delay (3000);

			Message1.FadeTo (0, 250);
			await Message2.FadeTo (0, 250);

			Message1.Text = "This could take moment !";
			Message2.Text = "";

			await Message1.FadeTo (100, 250);

			viewModel.FinishedShowingMessages = true;
			if(viewModel.FinishedLoadingAppointments)
			{
				await Message1.FadeTo (0, 250);
			    Message1.Text = "All Done !";
				await Message1.FadeTo (100, 250);
				viewModel.ShowStartButton = true;
			}
		}
    }
}
