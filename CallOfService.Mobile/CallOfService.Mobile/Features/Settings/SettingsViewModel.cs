using CallOfService.Mobile.UI;

namespace CallOfService.Mobile.Features.Settings
{
    public class SettingsViewModel : ViewModelBase
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
    }
}
