using CallOfService.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Settings
{
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
            InitializeComponent();
			NavigationPage.SetHasNavigationBar (this, true);
        }
    }
}
