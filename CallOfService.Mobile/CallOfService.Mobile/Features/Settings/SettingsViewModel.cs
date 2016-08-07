using CallOfService.Mobile.UI;
using PropertyChanged;

namespace CallOfService.Mobile.Features.Settings
{
    [ImplementPropertyChanged]
    public class SettingsViewModel : IViewAwareViewModel
    {
        public bool SendDeviceLocation
        {
            get
            {
                return Helpers.Settings.EnableLocation;
            }
            set
            {
                if (Helpers.Settings.EnableLocation == value)
                    return;

                Helpers.Settings.EnableLocation = value;
            }
        }

        public void Dispose()
        {
        }

        public void OnAppearing()
        {
        }

        public void OnDisappearing()
        {
        }
    }
}
