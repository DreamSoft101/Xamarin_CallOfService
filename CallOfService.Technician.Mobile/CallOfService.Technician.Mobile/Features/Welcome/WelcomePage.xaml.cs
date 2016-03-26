using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.Welcome
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
			NavigationPage.SetHasNavigationBar (this, false);
        }
    }
}
