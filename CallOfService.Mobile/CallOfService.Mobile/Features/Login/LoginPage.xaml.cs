using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Login
{
    public partial class LoginPage : BasePage
    {
        public LoginPage()
        {
            InitializeComponent();
			NavigationPage.SetHasNavigationBar (this, false);
        }
    }
}
